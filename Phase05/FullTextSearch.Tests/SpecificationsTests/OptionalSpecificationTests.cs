using FluentAssertions;
using FullTextSearch.InvertedIndexDs.Dtos;
using FullTextSearch.InvertedIndexDs.FilterSpecifications;
using FullTextSearch.InvertedIndexDs.QueryBuilder;
using FullTextSearch.InvertedIndexDs.QueryBuilder.Abstractions;
using FullTextSearch.InvertedIndexDs.SearchFeatures;
using FullTextSearch.InvertedIndexDs.SearchFeatures.Abstractions;
using NSubstitute;

namespace FullTextSearch.Tests.SpecificationsTests;

public class OptionalSpecificationTests
{

    private readonly ISearch _simpleSearch;
    private readonly IQueryExtractor _queryExtractor;
    private readonly string _query;
    private readonly InvertedIndexDto _dto;
    
    public OptionalSpecificationTests()
    {
        _simpleSearch = Substitute.For<ISearch>();
        _queryExtractor = Substitute.For<IQueryExtractor>();
        _dto =  Substitute.For<InvertedIndexDto>();
        _query = "get help +illness +disease -cough";
    }
    
    [Fact]
    public void Constructor_ShouldInitializeWithValidDependencies()
    {
        
        
        _queryExtractor.ExtractQueries(_query, @"^\+\w+")
            .Returns(new List<string> { "ILLNESS", "DISEASE" });
        
        var spec = new OptionalSpecification(_simpleSearch, _queryExtractor);
        
        spec.Should().NotBeNull();
    }
    
    
    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenSearchIsNull()
    {
        
        Action act = () => new OptionalSpecification(null, _queryExtractor);

        
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'simpleSearch')");
    }
    
    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenQueryExtractorIsNull()
    {
        
        Action act = () => new OptionalSpecification(_simpleSearch, null);

        
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'queryExtractor')");
    }
    
    [Fact]
    public void FilterDocumentsByQuery_ShouldUnionWithDocuments_WithSearchResults()
    {
        
        var expectedKeywords = new List<string> { "ILLNESS", "DISEASE" };
        _queryExtractor.ExtractQueries(_query, @"^\+\w+")
            .Returns(expectedKeywords);

        _simpleSearch.Search("ILLNESS", _dto).Returns(new SortedSet<string> { "doc1", "doc2", "doc3" });
        _simpleSearch.Search("DISEASE", _dto).Returns(new SortedSet<string> { "doc2", "doc3", "doc4" });

        var documents = new SortedSet<string> { "doc1", "doc2", "doc3", "doc5", "doc4"};
        
        var spec = new OptionalSpecification(_simpleSearch, _queryExtractor);
        spec.FilterDocumentsByQuery(documents, _query, _dto);
        
        documents.Should().BeEquivalentTo(new[] { "doc2", "doc3", "doc1", "doc4"}); 
    }
    
}