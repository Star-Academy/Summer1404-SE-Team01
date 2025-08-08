using FluentAssertions;
using FullTextSearch.InvertedIndex.Dtos;
using FullTextSearch.InvertedIndex.FilterStrategies;
using FullTextSearch.InvertedIndex.FilterStrategies.Abstractions;
using FullTextSearch.InvertedIndex.SearchFeatures.Abstractions;
using NSubstitute;


namespace FullTextSearch.Tests.FilterStrategyTests;

public class OptionalStrategyTests
{
    private readonly ISearch _search;
    private readonly IFilterStrategy _sut;

    public OptionalStrategyTests()
    {
        _search = Substitute.For<ISearch>();
        _sut = new OptionalStrategy(_search);
    }

    private static QueryDto CreateSampleQueryDto()
    {
        return new QueryDto
        {
            Optional = new List<string> { "ILLNESS", "DISEASE" },
            Required = new List<string> { "GET", "HELP" },
            Excluded = new List<string> { "COUGH", "STAR" }
        };
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenSearchIsNull()
    {
        Action act = () => new OptionalStrategy(null);

        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'searchService')");
    }

    [Fact]
    public void FilterDocumentsByQuery_ShouldReturnUnionOfDocuments_WithSearchResults()
    {
        // Arrange

        var dto = new InvertedIndexDto
        {
            AllDocuments = ["doc1", "doc2", "doc3", "doc4", "doc5"],
            InvertedIndexMap = []
        };

        _search.Search("ILLNESS", dto).Returns(["doc1", "doc2"]);
        _search.Search("DISEASE", dto).Returns(["doc2", "doc3", "doc4"]);

        var queryDto = CreateSampleQueryDto();

        // Act
        var expected = _sut.FilterDocumentsByQuery(queryDto, dto);

        // Assert
        expected.Should().BeEquivalentTo(["doc1", "doc2", "doc3", "doc4"]);
    }

    [Fact]
    public void FilterDocumentsByQuery_ShouldReturnEmptySet_WhenOptionalListEmpty()
    {
        // Arrange
        var queryDto = new QueryDto()
        {
            Optional = []
        };

        var dto = new InvertedIndexDto
        {
            AllDocuments = ["doc1", "doc2"],
            InvertedIndexMap = []
        };


        // Act
        var expected = _sut.FilterDocumentsByQuery(queryDto, dto);

        // Assert
        expected.Should().BeEquivalentTo(dto.AllDocuments);
        _search.DidNotReceive().Search(Arg.Any<string>(), Arg.Any<InvertedIndexDto>());
    }

    [Fact]
    public void FilterDocumentsByQuery_ShouldReturnEmptySet_WhenAllDocumentsIsEmpty()
    {
        // Arrange

        var dto = new InvertedIndexDto
        {
            AllDocuments = [],
            InvertedIndexMap = []
        };

        _search.Search(Arg.Any<string>(), dto).Returns([]);

        var queryDto = CreateSampleQueryDto();

        // Act
        var expected = _sut.FilterDocumentsByQuery(queryDto, dto);

        // Assert
        expected.Should().BeEmpty();
    }
}