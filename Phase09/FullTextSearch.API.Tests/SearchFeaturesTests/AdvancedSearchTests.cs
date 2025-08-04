using FluentAssertions;
using FullTextSearch.API.InvertedIndex.Dtos;
using FullTextSearch.API.InvertedIndex.FilterStrategies.Abstractions;
using FullTextSearch.API.InvertedIndex.SearchFeatures;
using NSubstitute;

namespace FullTextSearch.API.Tests.SearchFeaturesTests;

public class AdvancedSearchTests
{
    private readonly IFilterStrategy _filter1;
    private readonly IFilterStrategy _filter2;
    private readonly AdvancedSearch _sut;
    private const string Query = "get help +illness +disease -cough";

    public AdvancedSearchTests()
    {
        _filter1 = Substitute.For<IFilterStrategy>();
        _filter2 = Substitute.For<IFilterStrategy>();
        _sut = new AdvancedSearch([_filter1, _filter2]);
    }

    [Fact]
    public void Search_ShouldApplyAllFiltersAndReturnIntersection_WhenMultipleFiltersExist()
    {
        // Arrange
        var dto = new InvertedIndexDto
        {
            AllDocuments = ["doc1", "doc2", "doc3", "doc4"],
            InvertedIndexMap = []
        };

        _filter1.FilterDocumentsByQuery(Query, dto).Returns(["doc1", "doc2", "doc3"]);
        _filter2.FilterDocumentsByQuery(Query, dto).Returns(["doc2", "doc3", "doc4"]);

        // Act
        var expected = _sut.Search(Query, dto);

        // Assert
        expected.Should().BeEquivalentTo(["doc2", "doc3"]);
        _filter1.Received(1).FilterDocumentsByQuery(Query, dto);
        _filter2.Received(1).FilterDocumentsByQuery(Query, dto);
    }

    [Fact]
    public void Search_ShouldReturnEmptySet_WhenNoDocumentsMatchAllFilters()
    {
        // Arrange
        var dto = new InvertedIndexDto
        {
            AllDocuments = ["doc1", "doc2", "doc3"],
            InvertedIndexMap = []
        };

        _filter1.FilterDocumentsByQuery(Query, dto).Returns(["doc1", "doc2"]);
        _filter2.FilterDocumentsByQuery(Query, dto).Returns(["doc3"]);

        // Act
        var expected = _sut.Search(Query, dto);

        // Assert
        expected.Should().BeEmpty();
    }

    [Fact]
    public void Search_ShouldReturnAllDocuments_WhenNoFiltersProvided()
    {
        // Arrange
        var dto = new InvertedIndexDto
        {
            AllDocuments = ["doc1", "doc2"],
            InvertedIndexMap = []
        };

        var sut = new AdvancedSearch([]);

        // Act
        var expected = sut.Search(Query, dto);

        // Assert
        expected.Should().BeEquivalentTo(dto.AllDocuments);
    }

    [Fact]
    public void Search_ShouldHandleEmptyInitialDocumentSet()
    {
        // Arrange
        var dto = new InvertedIndexDto
        {
            AllDocuments = [],
            InvertedIndexMap = []
        };
        _filter1.FilterDocumentsByQuery(Arg.Any<string>(), Arg.Any<InvertedIndexDto>()).Returns([]);
        _filter2.FilterDocumentsByQuery(Arg.Any<string>(), Arg.Any<InvertedIndexDto>()).Returns([]);

        // Act
        var expected = _sut.Search(Query, dto);

        // Assert
        expected.Should().BeEmpty();
    }
}