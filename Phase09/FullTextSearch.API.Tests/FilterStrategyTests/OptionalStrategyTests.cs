using FluentAssertions;
using FullTextSearch.API.InvertedIndex.Dtos;
using FullTextSearch.API.InvertedIndex.FilterStrategies;
using FullTextSearch.API.InvertedIndex.SearchFeatures.Abstractions;
using NSubstitute;


namespace FullTextSearch.API.Tests.FilterStrategyTests;

public class OptionalStrategyTests
{
    private readonly ISearch _search;


    public OptionalStrategyTests()
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
        _search.Search("DISEASE", dto).Returns(["doc2", "doc3"]);
        _search.Search("optional phrase included".ToUpper(), dto).Returns(["doc3", "doc4"]);

        var queryDto = CreateSampleQueryDto();
        var sut = new OptionalStrategy(_search);

        // Act
        var expected = sut.FilterDocumentsByQuery(queryDto, dto);

        // Assert
        expected.Should().BeEquivalentTo(["doc1", "doc2", "doc3", "doc4"]);
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
            Optional = []
        };
        var sut = new OptionalStrategy(_search);

        // Act
        var expected = sut.FilterDocumentsByQuery(queryDto, dto);

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
        var sut = new OptionalStrategy(_search);

        // Act
        var expected = sut.FilterDocumentsByQuery(queryDto, dto);

        // Assert
        expected.Should().BeEmpty();
    }
}