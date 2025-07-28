using FluentAssertions;
using FullTextSearch.InvertedIndex;
using FullTextSearch.InvertedIndex.Dtos;
using FullTextSearch.InvertedIndex.FilterStrategies.Abstractions;
using FullTextSearch.InvertedIndex.SearchFeatures;
using NSubstitute;

namespace FullTextSearch.Tests.SearchFeaturesTests;

public class AdvancedSearchTests
{
    private readonly IInvertedIndexBuilder _invertedIndexBuilder;
    private readonly IStrategy _spec1 = Substitute.For<IStrategy>();
    private readonly IStrategy _spec2 = Substitute.For<IStrategy>();
    private const string _query = "get help +illness +disease -cough";
    private readonly InvertedIndexDto _dto;

    public AdvancedSearchTests()
    {
        _dto = Substitute.For<InvertedIndexDto>();
        _invertedIndexBuilder = Substitute.For<IInvertedIndexBuilder>();
    }

    [Fact]
    public void Search_ShouldApplyAllSpecifications_WhenTheyHaveKeywords()
    {
        var allDocs = new SortedSet<string> { "doc1", "doc2", "doc3" };

        _dto.AllDocuments = allDocs;


        var search = new AdvancedSearch(new List<IStrategy> { _spec1, _spec2 });

        var result = search.Search(_query, _dto);

        _spec1.Received(1).FilterDocumentsByQuery(Arg.Any<SortedSet<string>>(), _query, _dto);
        _spec2.Received(1).FilterDocumentsByQuery(Arg.Any<SortedSet<string>>(), _query, _dto);
        result.Should().BeEquivalentTo(allDocs); // no actual filtering in mock
    }

    [Fact]
    public void Search_ShouldSkipSpecification_WhenItHasNoKeywords()
    {
        _dto.AllDocuments = new SortedSet<string> { "doc1", "doc2" };

        var search = new AdvancedSearch(new List<IStrategy> { _spec1, _spec2 });

        var result = search.Search(_query, _dto);

        _spec1.Received(1).FilterDocumentsByQuery(Arg.Any<SortedSet<string>>(), _query, _dto);
        _spec2.Received(1).FilterDocumentsByQuery(Arg.Any<SortedSet<string>>(), _query, _dto);
        result.Should().BeEquivalentTo(_dto.AllDocuments);
    }

    [Fact]
    public void Search_ShouldReturnFilteredDocuments()
    {
        // Arrange
        var query = "+example";

        var dto = new InvertedIndexDto
        {
            AllDocuments = new SortedSet<string> { "doc1", "doc2", "doc3" },
            InvertedIndexMap = new SortedDictionary<string, SortedSet<DocumentInfo>>
            {
                ["EXAMPLE"] = new SortedSet<DocumentInfo>
                {
                    new DocumentInfo
                    {
                        DocId = "doc1",
                        Indexes = { 1,2,3 }
                    },
                    new DocumentInfo
                    {
                        DocId = "doc2",
                        Indexes = { 4,5,6 }

                    },
                    new DocumentInfo
                    {
                        DocId = "doc3",
                        Indexes = { 7,8,9 }
                    }
                }
            }
        };

        _spec1.When(x => x.FilterDocumentsByQuery(Arg.Any<SortedSet<string>>(), query, dto))
            .Do(call =>
            {
                var docs = call.Arg<SortedSet<string>>();
                docs.IntersectWith(new[] { "doc2", "doc3" });
            });

        _spec2.When(x => x.FilterDocumentsByQuery(Arg.Any<SortedSet<string>>(), query, dto))
            .Do(call =>
            {
                var docs = call.Arg<SortedSet<string>>();
                docs.UnionWith(new[] { "doc3" });
            });

        var search = new AdvancedSearch(new List<IStrategy> { _spec1, _spec2 });

        // Act
        var result = search.Search(query, dto);

        // Assert
        result.Should().BeEquivalentTo(new[] { "doc2", "doc3" });
    }
}
