using FluentAssertions;
using FullTextSearch.InvertedIndexDs;
using FullTextSearch.InvertedIndexDs.SearchFeatures;
using FullTextSearch.Services.TokenizerService;
using NSubstitute;


namespace FullTextSearch.Tests.SearchFeaturesTests;

public class InvertedIndexSimpleSearchTests
{
    private readonly InvertedIndexSimpleSearch _search;

    public InvertedIndexSimpleSearchTests()
    {
        var tokenizer = Substitute.For<ITokenizer>();
        var invertedIndex = new InvertedIndex(tokenizer);

        invertedIndex.InvertedIndexMap["APPLE"] = new SortedSet<string> { "doc1", "doc2" };
        invertedIndex.InvertedIndexMap["BANANA"] = new SortedSet<string> { "doc3" };

        _search = new InvertedIndexSimpleSearch(invertedIndex);
    }

    [Fact]
    public void Search_ShouldReturnMatchingDocuments_WhenWordExists()
    {
        var result = _search.Search("apple");

        result.Should().BeEquivalentTo(new[] { "doc1", "doc2" });
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

        result.Should().BeEquivalentTo(new[] { "doc1", "doc2" });
    }
}