using FluentAssertions;
using FullTextSearch.InvertedIndex.Dtos;
using FullTextSearch.InvertedIndex.QueryBuilder.Abstractions;
using FullTextSearch.InvertedIndex.SearchFeatures.Abstractions;
using FullTextSearch.InvertedIndexDs.FilterSpecifications;
using FullTextSearch.InvertedIndexDs.QueryBuilder;
using FullTextSearch.InvertedIndexDs.SearchFeatures;
using NSubstitute;

namespace FullTextSearch.Tests.SpecificationsTests;

public class ExcludedSpecificationTests
{

    private readonly ISearch _simpleSearch;
    private readonly IQueryExtractor _queryExtractor;
    private readonly string _query;
    private readonly InvertedIndexDto _dto;
    public ExcludedSpecificationTests()
    {
        _simpleSearch = Substitute.For<ISearch>();
        _queryExtractor = Substitute.For<IQueryExtractor>();
        _dto = Substitute.For<InvertedIndexDto>();
        _query = "get help +illness +disease -cough -star";
    }
    
    [Fact]
    public void Constructor_ShouldInitializeWithValidDependencies()
    {
        
        
        _queryExtractor.ExtractQueries(_query, @"^\-\w+")
            .Returns(new List<string> {"COUGH", "STAR"});
        
        var spec = new ExcludedStrategy(_simpleSearch, _queryExtractor);
        
        spec.Should().NotBeNull();
    }
    
    
    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenSearchIsNull()
    {
        
        Action act = () => new ExcludedStrategy(null, _queryExtractor);

        
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'simpleSearch')");
    }
    
    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenQueryExtractorIsNull()
    {
        
        Action act = () => new ExcludedStrategy(_simpleSearch, null);

        
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'queryExtractor')");
    }
    
    [Fact]
    public void FilterDocumentsByQuery_ShouldExcludeDocuments_WithSearchResults()
    {
        
        var expectedKeywords = new List<string> {"COUGH", "STAR"};
        _queryExtractor.ExtractQueries(_query, @"^\-\w+")
            .Returns(expectedKeywords);

        _simpleSearch.Search("COUGH", _dto).Returns(new SortedSet<string> { "doc1", "doc2", "doc3" });
        _simpleSearch.Search("STAR", _dto).Returns(new SortedSet<string> { "doc1", "doc2", "doc4" });

        var documents = new SortedSet<string> { "doc1", "doc2", "doc3", "doc5", "doc4" };

        var spec = new ExcludedStrategy(_simpleSearch, _queryExtractor);
        
        spec.FilterDocumentsByQuery(documents, _query, _dto);
        
        documents.Should().BeEquivalentTo(new[] { "doc5" }); 
    }
    
        
}