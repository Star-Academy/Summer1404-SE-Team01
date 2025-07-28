using FluentAssertions;
using FullTextSearch.InvertedIndexDs;
using FullTextSearch.InvertedIndexDs.Dtos;
using FullTextSearch.InvertedIndexDs.SearchFeatures;
using FullTextSearch.InvertedIndexDs.SearchFeatures.Abstractions;
using FullTextSearch.Services.TokenizerService;
using NSubstitute;


namespace FullTextSearch.Tests.SearchFeaturesTests;

public class InvertedIndexSimpleSearchTests
{
    private readonly SortedSet<DocumentInfo> appleDocInfos;
    private readonly ISearch _search;
    private readonly InvertedIndexDto _dto;

    public InvertedIndexSimpleSearchTests()
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

        _search = new InvertedIndexSimpleSearch();
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