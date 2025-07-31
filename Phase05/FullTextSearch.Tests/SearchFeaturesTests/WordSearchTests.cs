using FluentAssertions;
using FullTextSearch.InvertedIndex.Dtos;
using FullTextSearch.InvertedIndex.SearchFeatures;
using FullTextSearch.InvertedIndex.SearchFeatures.Abstractions;


namespace FullTextSearch.Tests.SearchFeaturesTests;

public class WordSearchTests
{
    private readonly SortedSet<DocumentInfo> appleDocInfos;
    private readonly ISearch _search;
    private readonly InvertedIndexDto _dto;

    public WordSearchTests()
    {
        _dto = new InvertedIndexDto
        {
            AllDocuments = new(),
            InvertedIndexMap = new()
        };

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

        _dto.InvertedIndexMap["APPLE"] = appleDocInfos;

        _search = new WordSearch();
    }

    [Fact]
    public void Search_ShouldReturnMatchingDocuments_WhenWordExists()
    {
        var result = _search.Search("apple", _dto);

        result.Should().BeEquivalentTo(appleDocInfos.Select(d => d.DocId));
    }

    [Fact]
    public void Search_ShouldReturnEmptySet_WhenWordNotExists()
    {
        var result = _search.Search("orange", _dto);

        result.Should().BeEmpty();
    }

    [Fact]
    public void Search_ShouldBeCaseInsensitive()
    {
        var result = _search.Search("ApPLe", _dto);

        result.Should().BeEquivalentTo(appleDocInfos.Select(d => d.DocId));
    }
}