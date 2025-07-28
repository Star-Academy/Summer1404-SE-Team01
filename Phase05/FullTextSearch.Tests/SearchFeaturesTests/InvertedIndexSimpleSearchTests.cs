using FluentAssertions;
using FullTextSearch.InvertedIndexDs;
using FullTextSearch.InvertedIndexDs.SearchFeatures;
using FullTextSearch.Services.TokenizerService;
using NSubstitute;


namespace FullTextSearch.Tests.SearchFeaturesTests;

public class InvertedIndexSimpleSearchTests
{
    private readonly InvertedIndexSimpleSearch _search;
    private static SortedSet<DocumentInfo> appleDocInfos;

    public InvertedIndexSimpleSearchTests()
    {
        var tokenizer = Substitute.For<ITokenizer>();
        var invertedIndex = new InvertedIndex(tokenizer);
        
        appleDocInfos = new SortedSet<DocumentInfo>
        {
            new DocumentInfo
            {
                DocId = "doc1",
                Indexes =
                {
                    50, 43, 44,
                }
            }, 
            new DocumentInfo
            {
                DocId = "doc2",
                Indexes =
                {
                    51, 99, 90
                }
            }
        };
        
        invertedIndex.InvertedIndexMap["APPLE"] = appleDocInfos;

        _search = new InvertedIndexSimpleSearch(invertedIndex);
    }

    [Fact]
    public void Search_ShouldReturnMatchingDocuments_WhenWordExists()
    {
        var result = _search.Search("apple");

        result.Should().BeEquivalentTo(appleDocInfos.Select(d => d.DocId));
    }

    [Fact]
    public void Search_ShouldReturnEmptySet_WhenWordNotExists()
    {
        var result = _search.Search("orange");

        result.Should().BeEmpty();
    }

    [Fact]
    public void Search_ShouldBeCaseInsensitive()
    {
        var result = _search.Search("ApPLe");

        result.Should().BeEquivalentTo(appleDocInfos.Select(d => d.DocId));
    }
}