using FluentAssertions;
using FullTextSearch.InvertedIndex.Dtos;
using FullTextSearch.InvertedIndex.FilterStrategies;
using FullTextSearch.InvertedIndex.SearchFeatures.Abstractions;
using NSubstitute;

namespace FullTextSearch.Tests.FilterStrategyTests;

public class ExcludedStrategyTests
{
    private readonly ISearch _search;

    public ExcludedStrategyTests()
    {
        _search = Substitute.For<ISearch>();
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
        Action act = () => new ExcludedStrategy(null);

        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'searchService')");
    }

    [Fact]
    public void FilterDocumentsByQuery_ShouldExcludeDocuments_WithSearchResults()
    {
        // Arrange
        var queryDto = CreateSampleQueryDto();

        var dto = new InvertedIndexDto
        {
            AllDocuments = ["doc1", "doc2", "doc3", "doc4", "doc5", "doc6"],
            InvertedIndexMap = []
        };

        _search.Search("COUGH", dto).Returns(["doc1"]);
        _search.Search("STAR", dto).Returns(["doc2", "doc3"]);

        var sut = new ExcludedStrategy(_search);

        // Act
        var result = sut.FilterDocumentsByQuery(queryDto, dto);

        // Assert
        result.Should().BeEquivalentTo(["doc4", "doc5", "doc6"]);
    }

    [Fact]
    public void FilterDocumentsByQuery_ShouldReturnAll_WhenExcludedListEmpty()
    {
        // Arrange
        var queryDto = new QueryDto
        {
            Excluded = []
        };

        var dto = new InvertedIndexDto
        {
            AllDocuments = ["doc1", "doc2"],
            InvertedIndexMap = []
        };

        var sut = new ExcludedStrategy(_search);

        // Act
        var result = sut.FilterDocumentsByQuery(queryDto, dto);

        // Assert
        result.Should().BeEquivalentTo(dto.AllDocuments);
        _search.DidNotReceive().Search(Arg.Any<string>(), Arg.Any<InvertedIndexDto>());
    }

    [Fact]
    public void FilterDocumentsByQuery_ShouldReturnEmpty_WhenAllDocumentsEmpty()
    {
        // Arrange
        var queryDto = CreateSampleQueryDto();

        var indexDto = new InvertedIndexDto
        {
            AllDocuments = [],
            InvertedIndexMap = []
        };

        _search.Search(Arg.Any<string>(), indexDto).Returns([]);

        var sut = new ExcludedStrategy(_search);

        // Act
        var result = sut.FilterDocumentsByQuery(queryDto, indexDto);

        // Assert
        result.Should().BeEmpty();
    }
}