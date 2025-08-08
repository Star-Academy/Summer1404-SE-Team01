using FluentAssertions;
using FullTextSearch.API.InvertedIndex.Dtos;
using FullTextSearch.API.InvertedIndex.FilterStrategies;
using FullTextSearch.API.InvertedIndex.SearchFeatures.Abstractions;
using NSubstitute;

namespace FullTextSearch.API.Tests.FilterStrategyTests;

public class RequiredStrategyTests
{
    private readonly ISearch _search;
    public RequiredStrategyTests()
    {
        _search = Substitute.For<ISearch>();
    }

    private static QueryDto CreateSampleQueryDto()
    {
        return new QueryDto
        {
            Optional = new List<string> { "ILLNESS", "DISEASE", "OPTIONAL PHRASE INCLUDED" },
            Required = new List<string> { "GET", "HELP", "HELLO WORLD PHRASE" },
            Excluded = new List<string> { "COUGH", "STAR" }
        };
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenSearchIsNull()
    {
        // Arrange & Act
        Action act = () => new RequiredStrategy(null);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'searchService')");
    }


    [Fact]
    public void FilterDocumentsByQuery_ShouldReturnIntersectionOfDocuments_WithSearchResults()
    {
        // Arrange

        var dto = new InvertedIndexDto
        {
            AllDocuments = ["doc1", "doc2", "doc3", "doc4", "doc5"],
            InvertedIndexMap = []

        };

        _search.Search("GET", dto).Returns(["doc1", "doc2"]);
        _search.Search("HELP", dto).Returns(["doc2", "doc3"]);
        _search.Search("HELLO WORLD PHRASE", dto).Returns(["doc2", "doc4"]);

        var queryDto = CreateSampleQueryDto();
        var sut = new RequiredStrategy(_search);

        // Act
        var result = sut.FilterDocumentsByQuery(queryDto, dto);

        // Assert
        result.Should().BeEquivalentTo(["doc2"]);
    }

    [Fact]
    public void FilterDocumentsByQuery_ShouldReturnEmptySet_WhenNoKeywordsFound()
    {
        // Arrange

        var dto = new InvertedIndexDto
        {
            AllDocuments = ["doc1", "doc2"],
            InvertedIndexMap = []

        };

        var queryDto = new QueryDto()
        {
            Required = []
        };
        var sut = new RequiredStrategy(_search);

        // Act
        var result = sut.FilterDocumentsByQuery(queryDto, dto);

        // Assert
        result.Should().BeEquivalentTo(dto.AllDocuments);
        _search.DidNotReceive().Search(Arg.Any<string>(), Arg.Any<InvertedIndexDto>());
    }

    [Fact]
    public void FilterDocumentsByQuery_ShouldReturnEmptySet_WhenAllDocumentsIsEmpty()
    {
        // Arrange

        var dto = new InvertedIndexDto
        {
            AllDocuments = new HashSet<string>(),
            InvertedIndexMap = []

        };

        _search.Search(Arg.Any<string>(), dto).Returns(["doc1"]);

        var queryDto = CreateSampleQueryDto();
        var sut = new RequiredStrategy(_search);

        // Act
        var result = sut.FilterDocumentsByQuery(queryDto, dto);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void FilterDocumentsByQuery_ShouldReturnEmptySet_WhenNoDocumentsMatchAllRequiredWords()
    {
        // Arrange

        var dto = new InvertedIndexDto
        {
            AllDocuments = ["doc1", "doc2", "doc3"],
            InvertedIndexMap = []

        };

        _search.Search("GET", dto).Returns(["doc1", "doc2"]);
        _search.Search("HELP", dto).Returns(["doc3"]);
        _search.Search("HELLO WORLD PHRASE", dto).Returns(["doc4"]);
        var queryDto = CreateSampleQueryDto();
        var sut = new RequiredStrategy(_search);

        // Act
        var result = sut.FilterDocumentsByQuery(queryDto, dto);

        // Assert
        result.Should().BeEmpty();
    }
}