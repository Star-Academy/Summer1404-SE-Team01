using FluentAssertions;
using FullTextSearch.InvertedIndexDs.Dtos;
using FullTextSearch.InvertedIndexDs.FilterSpecifications;
using FullTextSearch.InvertedIndexDs.QueryBuilder;
using FullTextSearch.InvertedIndexDs.QueryBuilder.Abstractions;
using FullTextSearch.InvertedIndexDs.SearchFeatures;
using FullTextSearch.InvertedIndexDs.SearchFeatures.Abstractions;
using NSubstitute;

namespace FullTextSearch.Tests.SpecificationsTests;

public class NecessarySpecificationTests
{

    private readonly ISearch _simpleSearch;
    private readonly IQueryExtractor _queryExtractor;
    private readonly string _query;
    private readonly InvertedIndexDto _dto;
    
    public NecessarySpecificationTests()
    {
        _simpleSearch = Substitute.For<ISearch>();
        _queryExtractor = Substitute.For<IQueryExtractor>();
        _dto = Substitute.For<InvertedIndexDto>();
        _query = "get help +illness +disease -cough";
    }
    
    [Fact]
    public void Constructor_ShouldInitializeWithValidDependencies()
    {
        
        
        _queryExtractor.ExtractQueries(_query, @"^[^-+]\w+")
            .Returns(new List<string> { "GET", "HELP" });
        
        var spec = new NecessarySpecification(_simpleSearch, _queryExtractor);
        
        spec.Should().NotBeNull();
    }
    
    
    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenSearchIsNull()
    {
        
        Action act = () => new NecessarySpecification(null, _queryExtractor);

        
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'simpleSearch')");
    }
    
    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenQueryExtractorIsNull()
    {
        
        Action act = () => new NecessarySpecification(_simpleSearch, null);

        
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'queryExtractor')");
    }
    
    [Fact]
    public void FilterDocumentsByQuery_ShouldIntersectDocuments_WithSearchResults()
    {
        
        var expectedKeywords = new List<string> { "GET", "HELP" };
        _queryExtractor.ExtractQueries(_query, @"^[^-+]\w+")
            .Returns(expectedKeywords);

        _simpleSearch.Search("GET",_dto).Returns(new SortedSet<string> { "doc1", "doc2", "doc3" });
        _simpleSearch.Search("HELP", _dto).Returns(new SortedSet<string> { "doc2", "doc3", "doc4" });

        var documents = new SortedSet<string> { "doc1", "doc2", "doc3", "doc5" };

        var spec =  new NecessarySpecification(_simpleSearch, _queryExtractor); 
        spec.FilterDocumentsByQuery(documents, _query, _dto);
        
        documents.Should().BeEquivalentTo(new[] { "doc2", "doc3" }); 
    }
    
        
}