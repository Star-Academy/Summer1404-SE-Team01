using FluentAssertions;
using FullTextSearch.InvertedIndex.Constants;
using FullTextSearch.InvertedIndex.Dtos;
using FullTextSearch.InvertedIndex.FilterStrategies;
using FullTextSearch.InvertedIndex.QueryBuilder.Abstractions;
using FullTextSearch.InvertedIndex.SearchFeatures.Abstractions;
using NSubstitute;

namespace FullTextSearch.Tests.FilterStrategyTests;

public class OptionalStrategyTests
{

    private readonly ISearch _search;
    private readonly IQueryExtractor _queryExtractor;
    private readonly string _query;
    private readonly InvertedIndexDto _dto;
    private readonly string _singleWordPattern;
    private readonly string _phrasePattern;

    public OptionalStrategyTests()
    {
        _search = Substitute.For<ISearch>();
        _queryExtractor = Substitute.For<IQueryExtractor>();
        _dto = Substitute.For<InvertedIndexDto>();
        _query = @"get help +illness +disease -cough -star ""hello world phrase"" +""optional phrase included"" ";
        _singleWordPattern = StrategyPatterns.OptionalSingleWord;
        _phrasePattern = StrategyPatterns.OptionalPhrase;
    }

    [Fact]
    public void Constructor_ShouldInitializeWithValidDependencies()
    {


        _queryExtractor.ExtractQueries(_query, _singleWordPattern)
            .Returns(new List<string> { "ILLNESS", "DISEASE" });

        var spec = new OptionalStrategy(_search, _queryExtractor, _singleWordPattern);

        spec.Should().NotBeNull();
    }


    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenSearchIsNull()
    {

        Action act = () => new OptionalStrategy(null, _queryExtractor, _singleWordPattern);


        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'simpleSearch')");
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenQueryExtractorIsNull()
    {

        Action act = () => new OptionalStrategy(_search, null, _singleWordPattern);


        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'queryExtractor')");
    }

    [Fact]
    public void FilterDocumentsByQuery_ShouldUnionWithDocuments_WithSearchResults()
    {

        var expectedKeywords = new List<string> { "ILLNESS", "DISEASE" };
        _queryExtractor.ExtractQueries(_query, @"^\+\w+")
            .Returns(expectedKeywords);

        _search.Search("ILLNESS", _dto).Returns(new SortedSet<string> { "doc1", "doc2", "doc3" });
        _search.Search("DISEASE", _dto).Returns(new SortedSet<string> { "doc2", "doc3", "doc4" });

        var documents = new SortedSet<string> { "doc1", "doc2", "doc3", "doc5", "doc4" };

        var spec = new OptionalStrategy(_search, _queryExtractor, _singleWordPattern);
        spec.FilterDocumentsByQuery(documents, _query, _dto);

        documents.Should().BeEquivalentTo(new[] { "doc2", "doc3", "doc1", "doc4" });
    }
    
    [Fact]
    public void FilterDocumentsByQuery_ShouldUnionWithDocuments_WithPhraseSearchResults()
    {
        var expectedExtractedPhrase = "optional phrase included".ToUpper();
        var expectedKeywords = new List<string> { expectedExtractedPhrase };
        _queryExtractor.ExtractQueries(_query, _phrasePattern)
            .Returns(expectedKeywords);

        
        _search.Search(expectedExtractedPhrase, _dto).Returns(new SortedSet<string> { "doc2", "doc3", "doc4" });

        var documents = new SortedSet<string> { "doc1", "doc2", "doc3", "doc5", "doc4" };

        var spec = new OptionalStrategy(_search, _queryExtractor, _phrasePattern);
        spec.FilterDocumentsByQuery(documents, _query, _dto);

        documents.Should().BeEquivalentTo(new[] { "doc2", "doc3", "doc4" });
    }

}