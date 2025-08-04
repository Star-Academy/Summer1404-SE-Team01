using FluentAssertions;
using FullTextSearch.API.InvertedIndex.Constants;
using FullTextSearch.API.InvertedIndex.Dtos;
using FullTextSearch.API.InvertedIndex.QueryBuilder.Abstractions;
using FullTextSearch.API.InvertedIndex.SearchFeatures.Abstractions;
using FullTextSearch.InvertedIndex.FilterStrategies;
using NSubstitute;

namespace FullTextSearch.API.Tests.FilterStrategyTests;

public class OptionalStrategyTests
{
    private readonly ISearch _search;
    private readonly IQueryExtractor _queryExtractor;
    private const string Query = @"get help +illness +disease -cough -star ""hello world phrase"" +""optional phrase included"" ";
    private const string SingleWordPattern = StrategyPatterns.OptionalSingleWord;
    private const string PhrasePattern = StrategyPatterns.OptionalPhrase;

    public OptionalStrategyTests()
    {
        _search = Substitute.For<ISearch>();
        _queryExtractor = Substitute.For<IQueryExtractor>();
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenSearchIsNull()
    {
        Action act = () => new OptionalStrategy(null, _queryExtractor, SingleWordPattern);

        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'searchType')");
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenQueryExtractorIsNull()
    {
        Action act = () => new OptionalStrategy(_search, null, SingleWordPattern);

        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'queryExtractor')");
    }

    [Fact]
    public void FilterDocumentsByQuery_ShouldReturnUnionOfDocuments_WithSearchResults()
    {
        // Arrange
        var expectedKeywords = new List<string> { "ILLNESS", "DISEASE" };
        _queryExtractor.ExtractQueries(Query, SingleWordPattern)
            .Returns(expectedKeywords);

        var dto = new InvertedIndexDto
        {
            AllDocuments = ["doc1", "doc2", "doc3", "doc4", "doc5"],
            InvertedIndexMap = []
        };

        _search.Search("ILLNESS", dto).Returns(["doc1", "doc2", "doc3"]);
        _search.Search("DISEASE", dto).Returns(["doc2", "doc3", "doc4"]);

        var sut = new OptionalStrategy(_search, _queryExtractor, SingleWordPattern);

        // Act
        var expected = sut.FilterDocumentsByQuery(Query, dto);

        // Assert
        expected.Should().BeEquivalentTo(["doc1", "doc2", "doc3", "doc4"]);
    }

    [Fact]
    public void FilterDocumentsByQuery_ShouldReturnUnionOfDocuments_WithPhraseSearchResults()
    {
        // Arrange
        var expectedExtractedPhrase = "optional phrase included".ToUpper();
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

        var sut = new OptionalStrategy(_search, _queryExtractor, PhrasePattern);

        // Act
        var expected = sut.FilterDocumentsByQuery(Query, dto);

        // Assert
        expected.Should().BeEquivalentTo(["doc2", "doc3", "doc4"]);
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

        var sut = new OptionalStrategy(_search, _queryExtractor, SingleWordPattern);

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
        var expectedKeywords = new List<string> { "ILLNESS" };
        _queryExtractor.ExtractQueries(Query, SingleWordPattern)
            .Returns(expectedKeywords);

        var dto = new InvertedIndexDto
        {
            AllDocuments = [],
            InvertedIndexMap = []
        };

        _search.Search("ILLNESS", dto).Returns(new SortedSet<string>());

        var sut = new OptionalStrategy(_search, _queryExtractor, SingleWordPattern);

        // Act
        var expected = sut.FilterDocumentsByQuery(Query, dto);

        // Assert
        expected.Should().BeEmpty();
    }
}