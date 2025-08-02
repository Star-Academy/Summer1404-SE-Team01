using FluentAssertions;
using FullTextSearch.InvertedIndex.Constants;
using FullTextSearch.InvertedIndex.Dtos;
using FullTextSearch.InvertedIndex.FilterStrategies;
using FullTextSearch.InvertedIndex.QueryBuilder.Abstractions;
using FullTextSearch.InvertedIndex.SearchFeatures.Abstractions;
using NSubstitute;

namespace FullTextSearch.Tests.FilterStrategyTests;

public class ExcludedStrategyTests
{
    private readonly ISearch _search;
    private readonly IQueryExtractor _queryExtractor;
    private const string Query = @"get help +illness +disease -cough -star ""hello world phrase"" -""excluded phrase included"" ";
    private const string SingleWordPattern = StrategyPatterns.ExcludedSingleWord;
    private const string PhrasePattern = StrategyPatterns.ExcludedPhrase;

    public ExcludedStrategyTests()
    {
        _search = Substitute.For<ISearch>();
        _queryExtractor = Substitute.For<IQueryExtractor>();
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenSearchIsNull()
    {
        Action act = () => new ExcludedStrategy(null, _queryExtractor, SingleWordPattern);

        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'searchType')");
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenQueryExtractorIsNull()
    {
        Action act = () => new ExcludedStrategy(_search, null, SingleWordPattern);

        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'queryExtractor')");
    }

    [Fact]
    public void FilterDocumentsByQuery_ShouldExcludeDocuments_WithSearchResults()
    {
        // Arrange
        var expectedKeywords = new List<string> { "COUGH", "STAR" };
        _queryExtractor.ExtractQueries(Query, SingleWordPattern)
            .Returns(expectedKeywords);

        var dto = new InvertedIndexDto
        {
            AllDocuments = ["doc1", "doc2", "doc3", "doc4", "doc5", "doc6"],
            InvertedIndexMap = []
        };

        _search.Search("COUGH", dto).Returns(["doc1", "doc2"]);
        _search.Search("STAR", dto).Returns(["doc3", "doc4"]);

        var sut = new ExcludedStrategy(_search, _queryExtractor, SingleWordPattern);

        // Act
        var expected = sut.FilterDocumentsByQuery(Query, dto);

        // Assert
        expected.Should().BeEquivalentTo(["doc5", "doc6"]);
    }

    [Fact]
    public void FilterDocumentsByQuery_ShouldExcludeDocuments_WithSearchPhraseResults()
    {
        // Arrange
        var expectedExtractedPhrase = "excluded phrase included".ToUpper();
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

        var sut = new ExcludedStrategy(_search, _queryExtractor, PhrasePattern);

        // Act
        var expected = sut.FilterDocumentsByQuery(Query, dto);

        // Assert
        expected.Should().BeEquivalentTo(["doc1", "doc5"]);
    }

    [Fact]
    public void FilterDocumentsByQuery_ShouldReturnAllDocuments_WhenNoKeywordsFound()
    {
        // Arrange
        _queryExtractor.ExtractQueries(Query, SingleWordPattern)
            .Returns(new List<string>());

        var dto = new InvertedIndexDto
        {
            AllDocuments = ["doc1", "doc2"],
            InvertedIndexMap = []
        };

        var sut = new ExcludedStrategy(_search, _queryExtractor, SingleWordPattern);

        // Act
        var expected = sut.FilterDocumentsByQuery(Query, dto);

        // Assert
        expected.Should().BeEquivalentTo(dto.AllDocuments);
        _search.DidNotReceive().Search(Arg.Any<string>(), Arg.Any<InvertedIndexDto>());
    }

    [Fact]
    public void FilterDocumentsByQuery_ShouldReturnEmptySet_WhenAllDocumentsIsEmpty()
    {
        // Arrange
        var expectedKeywords = new List<string> { "COUGH" };
        _queryExtractor.ExtractQueries(Query, SingleWordPattern)
            .Returns(expectedKeywords);

        var dto = new InvertedIndexDto
        {
            AllDocuments = [],
            InvertedIndexMap = []
        };

        _search.Search(Arg.Any<string>(), dto).Returns([]);

        var sut = new ExcludedStrategy(_search, _queryExtractor, SingleWordPattern);

        // Act
        var expected = sut.FilterDocumentsByQuery(Query, dto);

        // Assert
        expected.Should().BeEmpty();
    }
}