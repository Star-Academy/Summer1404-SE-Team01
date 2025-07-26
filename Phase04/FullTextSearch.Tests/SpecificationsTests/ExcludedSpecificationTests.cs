using FluentAssertions;
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
    public ExcludedSpecificationTests()
    {
        _simpleSearch = Substitute.For<ISearch>();
        _queryExtractor = Substitute.For<IQueryExtractor>();
        _query = "get help +illness +disease -cough -star";
    }
    
    [Fact]
    public void Constructor_ShouldInitializeWithValidDependencies()
    {
        
        
        _queryExtractor.ExtractQueries(_query, @"^\-\w+")
            .Returns(new List<string> {"COUGH", "STAR"});
        
        var spec = new ExcludedSpecification(_simpleSearch, _queryExtractor, _query);
        
        spec.Should().NotBeNull();
        spec.Keywords.Should().BeEquivalentTo(new[] {"COUGH", "STAR"});
    }
    
    
    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenSearchIsNull()
    {
        
        Action act = () => new ExcludedSpecification(null, _queryExtractor, _query);

        
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'simpleSearch')");
    }
    
    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenQueryExtractorIsNull()
    {
        
        Action act = () => new ExcludedSpecification(_simpleSearch, null, _query);

        
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'queryExtractor')");
    }
    
    [Fact]
    public void FilterDocumentsByQuery_ShouldExcludeDocuments_WithSearchResults()
    {
        
        var expectedKeywords = new List<string> {"COUGH", "STAR"};
        _queryExtractor.ExtractQueries(_query, @"^\-\w+")
            .Returns(expectedKeywords);

        _simpleSearch.Search("COUGH").Returns(new SortedSet<string> { "doc1", "doc2", "doc3" });
        _simpleSearch.Search("STAR").Returns(new SortedSet<string> { "doc1", "doc2", "doc4" });

        var documents = new SortedSet<string> { "doc1", "doc2", "doc3", "doc5", "doc4" };

        var spec = new ExcludedSpecification(_simpleSearch, _queryExtractor, _query);

        
        spec.FilterDocumentsByQuery(documents);
        
        documents.Should().BeEquivalentTo(new[] { "doc5" }); 
    }
    
        
}