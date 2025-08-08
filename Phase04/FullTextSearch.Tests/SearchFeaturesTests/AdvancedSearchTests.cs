using FluentAssertions;
using FullTextSearch.InvertedIndex.Dtos;
using FullTextSearch.InvertedIndex.FilterStrategies.Abstractions;
using FullTextSearch.InvertedIndex.SearchFeatures;
using NSubstitute;

namespace FullTextSearch.Tests.SearchFeaturesTests;

public class AdvancedSearchTests
{
    private readonly IFilterStrategy _filter1;
    private readonly IFilterStrategy _filter2;
    private readonly AdvancedSearch _sut;


    public static QueryDto CreateSampleQueryDto()
    {
        return new QueryDto
        {
            Optional = new List<string> { "ILLNESS", "DISEASE" },
            Required = new List<string> { "GET", "HELP" },
            Excluded = new List<string> { "COUGH", "STAR" }
        };
    }
    public AdvancedSearchTests()
    {
        _filter1 = Substitute.For<IFilterStrategy>();
        _filter2 = Substitute.For<IFilterStrategy>();
        _sut = new AdvancedSearch();
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

        var querDto = CreateSampleQueryDto();
        _filter1.FilterDocumentsByQuery(querDto, dto).Returns(["doc1", "doc2", "doc3"]);
        _filter2.FilterDocumentsByQuery(querDto, dto).Returns(["doc2", "doc3", "doc4"]);

        // Act
        var expected = _sut.Search(querDto, dto, new List<IFilterStrategy> { _filter1, _filter2 });

        // Assert
        expected.Should().BeEquivalentTo(["doc2", "doc3"]);
        _filter1.Received(1).FilterDocumentsByQuery(querDto, dto);
        _filter2.Received(1).FilterDocumentsByQuery(querDto, dto);
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

        var queryDto = CreateSampleQueryDto();
        _filter1.FilterDocumentsByQuery(queryDto, dto).Returns(["doc1", "doc2"]);
        _filter2.FilterDocumentsByQuery(queryDto, dto).Returns(["doc3"]);

        // Act
        var expected = _sut.Search(queryDto, dto, new List<IFilterStrategy> { _filter1, _filter2 });

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

        var sut = new AdvancedSearch();
        var queryDto = CreateSampleQueryDto();

        // Act
        var expected = sut.Search(queryDto, dto, new List<IFilterStrategy>());

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
        _filter1.FilterDocumentsByQuery(Arg.Any<QueryDto>(), Arg.Any<InvertedIndexDto>()).Returns([]);
        _filter2.FilterDocumentsByQuery(Arg.Any<QueryDto>(), Arg.Any<InvertedIndexDto>()).Returns([]);
        var queryDto = CreateSampleQueryDto();

        // Act
        var expected = _sut.Search(queryDto, dto, new List<IFilterStrategy> { _filter1, _filter2 });

        // Assert
        expected.Should().BeEmpty();
    }
}