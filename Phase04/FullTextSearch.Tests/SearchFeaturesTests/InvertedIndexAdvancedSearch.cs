using FluentAssertions;
using FullTextSearch.InvertedIndexDs;
using NSubstitute;
using FullTextSearch.InvertedIndexDs.FilterSpecifications;
using FullTextSearch.InvertedIndexDs.SearchFeatures;

namespace FullTextSearch.Tests.SearchFeaturesTests;

public class InvertedIndexAdvancedSearchTests
{
    private readonly IInvertedIndexBuilder _invertedIndex = Substitute.For<IInvertedIndexBuilder>();
    private readonly ISpecification _spec1 = Substitute.For<ISpecification>();
    private readonly ISpecification _spec2 = Substitute.For<ISpecification>();
    private readonly string _query = "get help +illness +disease -cough"; 
    

    [Fact]
    public void Search_ShouldApplyAllSpecifications_WhenTheyHaveKeywords()
    {
        var allDocs = new SortedSet<string> { "doc1", "doc2", "doc3" };
        _invertedIndex.AllDocuments.Returns(allDocs);

        _spec1.Keywords.Returns(new List<string> { "one" });
        _spec2.Keywords.Returns(new List<string> { "two" });

        var search = new InvertedIndexAdvancedSearch(_invertedIndex, new List<ISpecification> { _spec1, _spec2 });

        var result = search.Search(_query);

        _spec1.Received(1).FilterDocumentsByQuery(Arg.Any<SortedSet<string>>());
        _spec2.Received(1).FilterDocumentsByQuery(Arg.Any<SortedSet<string>>());
        result.Should().BeEquivalentTo(allDocs); // no actual filtering in mock
    }

    [Fact]
    public void Search_ShouldSkipSpecification_WhenItHasNoKeywords()
    {
        var allDocs = new SortedSet<string> { "doc1", "doc2" };
        _invertedIndex.AllDocuments.Returns(allDocs);

        _spec1.Keywords.Returns(new List<string> { "valid" });
        _spec2.Keywords.Returns(new List<string>()); // empty -> should skip

        var search = new InvertedIndexAdvancedSearch(_invertedIndex, new List<ISpecification> { _spec1, _spec2 });

        var result = search.Search(_query);

        _spec1.Received(1).FilterDocumentsByQuery(Arg.Any<SortedSet<string>>());
        _spec2.DidNotReceive().FilterDocumentsByQuery(Arg.Any<SortedSet<string>>());
        result.Should().BeEquivalentTo(allDocs);
    }
    
    [Fact]
    public void Search_ShouldReturnFilteredDocuments()
    {
        
        var initialDocs = new SortedSet<string> { "doc1", "doc2", "doc3" };
        _invertedIndex.AllDocuments.Returns(initialDocs);

        _spec1.Keywords.Returns(new List<string> { "APPLE" });
        _spec1.When(x => x.FilterDocumentsByQuery(Arg.Any<SortedSet<string>>()))
              .Do(call =>
              {
                  var docs = call.Arg<SortedSet<string>>();
                  docs.IntersectWith(new[] { "doc2", "doc3" });  // find intersect for example
              });

        _spec2.Keywords.Returns(new List<string> { "BANANA" });
        _spec2.When(x => x.FilterDocumentsByQuery(Arg.Any<SortedSet<string>>()))
              .Do(call =>
              {
                  var docs = call.Arg<SortedSet<string>>();
                  docs.UnionWith(new[] { "doc3" });  // find union for example
              });

        var search = new InvertedIndexAdvancedSearch(_invertedIndex, new List<ISpecification> { _spec1, _spec2 });

        var result = search.Search("+example");
        
        result.Should().BeEquivalentTo(new[] { "doc3", "doc2" });
    }
}
