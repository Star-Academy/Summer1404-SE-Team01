using FluentAssertions;
using FullTextSearch.InvertedIndex.Constants;
using FullTextSearch.InvertedIndex.Dtos;
using FullTextSearch.InvertedIndex.FilterStrategies;
using FullTextSearch.InvertedIndex.SearchFeatures.Abstractions;
using FullTextSearch.Services.QueryBuilder.Abstractions;
using NSubstitute;

namespace FullTextSearch.Tests.FilterStrategyTests;

public class RequiredStrategyTests
{
    private readonly ISearch _search;
    private readonly IQueryExtractor _queryExtractor;
    private const string Query = @"get help +illness +disease -cough -star ""hello world phrase"" +""optional phrase included"" ";
    private const string SingleWordPattern = StrategyPatterns.RequiredSingleWord;
    private const string PhrasePattern = StrategyPatterns.RequiredPhrase;

    public RequiredStrategyTests()
    {
        _search = Substitute.For<ISearch>();
        _queryExtractor = Substitute.For<IQueryExtractor>();
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenSearchIsNull()
    {
        // Arrange & Act
        Action act = () => new RequiredStrategy(null, _queryExtractor, SingleWordPattern);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'searchType')");
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenQueryExtractorIsNull()
    {
        // Arrange & Act
        Action act = () => new RequiredStrategy(_search, null, SingleWordPattern);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'queryExtractor')");
    }

    [Fact]
    public void FilterDocumentsByQuery_ShouldReturnIntersectionOfDocuments_WithSearchResults()
    {
        // Arrange
        var expectedKeywords = new List<string> { "GET", "HELP" };
        _queryExtractor.ExtractQueries(Query, SingleWordPattern)
            .Returns(expectedKeywords);

        var dto = new InvertedIndexDto
        {
            AllDocuments = ["doc1", "doc2", "doc3", "doc4", "doc5"],
            InvertedIndexMap = []

        };

        _search.Search("GET", dto).Returns(["doc1", "doc2", "doc3"]);
        _search.Search("HELP", dto).Returns(["doc2", "doc3", "doc4"]);

        var sut = new RequiredStrategy(_search, _queryExtractor, SingleWordPattern);

        // Act
        var result = sut.FilterDocumentsByQuery(Query, dto);

        // Assert
        result.Should().BeEquivalentTo(["doc2", "doc3"]);
    }

    [Fact]
    public void FilterDocumentsByQuery_ShouldReturnIntersectionOfDocuments_WithPhraseSearchResults()
    {
        // Arrange
        var expectedExtractedPhrase = "hello world phrase".ToUpper();
        var expectedKeywords = new List<string> { expectedExtractedPhrase };
        _queryExtractor.ExtractQueries(Query, PhrasePattern)
            .Returns(expectedKeywords);

        var dto = new InvertedIndexDto
        {
            AllDocuments = ["doc1", "doc2", "doc3", "doc4", "doc5"],
            InvertedIndexMap = []

        };

        _search.Search(expectedExtractedPhrase, dto)
            .Returns(["doc2", "doc3", "doc4"]);

        var sut = new RequiredStrategy(_search, _queryExtractor, PhrasePattern);

        // Act
        var result = sut.FilterDocumentsByQuery(Query, dto);

        // Assert
        result.Should().BeEquivalentTo(["doc2", "doc3", "doc4"]);
    }

    [Fact]
    public void FilterDocumentsByQuery_ShouldReturnEmptySet_WhenNoKeywordsFound()
    {
        // Arrange
        _queryExtractor.ExtractQueries(Query, SingleWordPattern)
            .Returns(new List<string>());

        var dto = new InvertedIndexDto
        {
            AllDocuments = ["doc1", "doc2"],
            InvertedIndexMap = []

        };

        var sut = new RequiredStrategy(_search, _queryExtractor, SingleWordPattern);

        // Act
        var result = sut.FilterDocumentsByQuery(Query, dto);

        // Assert
        result.Should().BeEquivalentTo(dto.AllDocuments);
        _search.DidNotReceive().Search(Arg.Any<string>(), Arg.Any<InvertedIndexDto>());
    }

    [Fact]
    public void FilterDocumentsByQuery_ShouldReturnEmptySet_WhenAllDocumentsIsEmpty()
    {
        // Arrange
        var expectedKeywords = new List<string> { "GET" };
        _queryExtractor.ExtractQueries(Query, SingleWordPattern)
            .Returns(expectedKeywords);

        var dto = new InvertedIndexDto
        {
            AllDocuments = new HashSet<string>(),
            InvertedIndexMap = []

        };

        _search.Search("GET", dto).Returns(["doc1"]);

        var sut = new RequiredStrategy(_search, _queryExtractor, SingleWordPattern);

        // Act
        var result = sut.FilterDocumentsByQuery(Query, dto);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void FilterDocumentsByQuery_ShouldReturnEmptySet_WhenNoDocumentsMatchAllRequiredWords()
    {
        // Arrange
        var expectedKeywords = new List<string> { "GET", "HELP" };
        _queryExtractor.ExtractQueries(Query, SingleWordPattern)
            .Returns(expectedKeywords);

        var dto = new InvertedIndexDto
        {
            AllDocuments = ["doc1", "doc2", "doc3"],
            InvertedIndexMap = []

        };

        _search.Search("GET", dto).Returns(["doc1", "doc2"]);
        _search.Search("HELP", dto).Returns(["doc3"]);

        var sut = new RequiredStrategy(_search, _queryExtractor, SingleWordPattern);

        // Act
        var result = sut.FilterDocumentsByQuery(Query, dto);

        // Assert
        result.Should().BeEmpty();
    }
}