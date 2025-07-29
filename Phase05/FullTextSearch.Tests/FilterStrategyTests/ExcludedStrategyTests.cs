using FluentAssertions;
using FullTextSearch.InvertedIndex.Dtos;
using FullTextSearch.InvertedIndex.FilterStrategies;
using FullTextSearch.InvertedIndex.QueryBuilder.Abstractions;
using FullTextSearch.InvertedIndex.SearchFeatures.Abstractions;
using FullTextSearch.InvertedIndex.Constants;
using NSubstitute;

namespace FullTextSearch.Tests.FilterStrategyTests;

public class ExcludedStrategyTests
{

    private readonly ISearch _search;
    private readonly IQueryExtractor _queryExtractor;
    private readonly string _query;
    private readonly InvertedIndexDto _dto;
    private readonly string _singleWordPattern;
    private readonly string _phrasePattern;
    
    public ExcludedStrategyTests()
    {
        _search = Substitute.For<ISearch>();
        _queryExtractor = Substitute.For<IQueryExtractor>();
        _dto = Substitute.For<InvertedIndexDto>();
        _query = @"get help +illness +disease -cough -star ""hello world phrase"" -""excluded phrase included"" ";
        _singleWordPattern = StrategyPatterns.ExcludedSingleWord;
        _phrasePattern = StrategyPatterns.ExcludedPhrase;
    }

    [Fact]
    public void Constructor_ShouldInitializeWithValidDependencies()
    {
        _queryExtractor.ExtractQueries(_query, _singleWordPattern)
            .Returns(new List<string> { "COUGH", "STAR" });

        var spec = new ExcludedStrategy(_search, _queryExtractor, _singleWordPattern);

        spec.Should().NotBeNull();
    }
    

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenSearchIsNull()
    {

        Action act = () => new ExcludedStrategy(null, _queryExtractor, _singleWordPattern);


        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'simpleSearch')");
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenQueryExtractorIsNull()
    {

        Action act = () => new ExcludedStrategy(_search, null, _singleWordPattern);


        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'queryExtractor')");
    }

    [Fact]
    public void FilterDocumentsByQuery_ShouldExcludeDocuments_WithSearchResults()
    {
        var expectedExtractedPhrase = "excluded phrase included".ToUpper();
        var expectedKeywords = new List<string> { expectedExtractedPhrase };
        _queryExtractor.ExtractQueries(_query, _phrasePattern)
            .Returns(expectedKeywords);

        
        _search.Search(expectedExtractedPhrase, _dto).Returns(new SortedSet<string> { "doc1", "doc2", "doc4" });

        var documents = new SortedSet<string> { "doc1", "doc2", "doc3", "doc4", "doc5" };

        var spec = new ExcludedStrategy(_search, _queryExtractor, _phrasePattern);

        spec.FilterDocumentsByQuery(documents, _query, _dto);

        documents.Should().BeEquivalentTo(new[] { "doc5", "doc3" });
    }


}