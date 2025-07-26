using FluentAssertions;
using FullTextSearch.InvertedIndexDs.FilterSpecifications;
using FullTextSearch.InvertedIndexDs.QueryBuilder;
using FullTextSearch.InvertedIndexDs.SearchFeatures;
using NSubstitute;

namespace FullTextSearch.Tests.SpecificationsTests;

public class OptionalSpecificationTests
{

    private readonly ISearch _simpleSearch;
    private readonly IQueryExtractor _queryExtractor;
    private readonly string _query;
    
    public OptionalSpecificationTests()
    {
        _simpleSearch = Substitute.For<ISearch>();
        _queryExtractor = Substitute.For<IQueryExtractor>();
        _query = "get help +illness +disease -cough";
    }
    
    [Fact]
    public void Constructor_ShouldInitializeWithValidDependencies()
    {
        
        
        _queryExtractor.ExtractQueries(_query, @"^\+\w+")
            .Returns(new List<string> { "ILLNESS", "DISEASE" });
        
        var spec = new OptionalSpecification(_simpleSearch, _queryExtractor, _query);
        
        spec.Should().NotBeNull();
        spec.Keywords.Should().BeEquivalentTo(new[] { "ILLNESS", "DISEASE" });
    }
    
    
    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenSearchIsNull()
    {
        
        Action act = () => new OptionalSpecification(null, _queryExtractor, _query);

        
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'simpleSearch')");
    }
    
    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenQueryExtractorIsNull()
    {
        
        Action act = () => new OptionalSpecification(_simpleSearch, null, _query);

        
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'queryExtractor')");
    }
    
    [Fact]
    public void FilterDocumentsByQuery_ShouldUnionWithDocuments_WithSearchResults()
    {
        
        var expectedKeywords = new List<string> { "ILLNESS", "DISEASE" };
        _queryExtractor.ExtractQueries(_query, @"^\+\w+")
            .Returns(expectedKeywords);

        _simpleSearch.Search("ILLNESS").Returns(new SortedSet<string> { "doc1", "doc2", "doc3" });
        _simpleSearch.Search("DISEASE").Returns(new SortedSet<string> { "doc2", "doc3", "doc4" });

        var documents = new SortedSet<string> { "doc1", "doc2", "doc3", "doc5", "doc4"};
        
        var spec = new OptionalSpecification(_simpleSearch, _queryExtractor, _query);
        spec.FilterDocumentsByQuery(documents);
        
        documents.Should().BeEquivalentTo(new[] { "doc2", "doc3", "doc1", "doc4"}); 
    }
    
}