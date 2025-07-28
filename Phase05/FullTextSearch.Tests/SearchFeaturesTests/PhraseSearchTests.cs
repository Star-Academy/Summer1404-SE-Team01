using FluentAssertions;
using FullTextSearch.InvertedIndex.Dtos;
using FullTextSearch.InvertedIndex.FilterStrategies.Abstractions;
using FullTextSearch.InvertedIndex.SearchFeatures;
using FullTextSearch.InvertedIndex.SearchFeatures.Abstractions;
using FullTextSearch.Services.TokenizerService;
using NSubstitute;

namespace FullTextSearch.Tests.SearchFeaturesTests;

public class PhraseSearchTests
{
    private readonly ITokenizer _tokenizer;
    private readonly ISearch _simpleSearch;
    private readonly ISequentialPhrase _sequentialPhraseCheck;
    private readonly PhraseSearch _phraseSearch;

    public PhraseSearchTests()
    {
        _tokenizer = Substitute.For<ITokenizer>();
        _simpleSearch = Substitute.For<ISearch>();
        _sequentialPhraseCheck = Substitute.For<ISequentialPhrase>();
        _phraseSearch = new PhraseSearch(_tokenizer, _simpleSearch, _sequentialPhraseCheck);
    }

    [Fact]
    public void Search_ShouldReturnCommonDocuments_AcrossAllWords()
    {
        var input = "code star";
        _tokenizer.Tokenize(input).Returns(new List<string> { "CODE", "STAR" });

        var dto = CreateDto();

        _simpleSearch.Search("CODE", dto).Returns(new SortedSet<string> { "doc1", "doc2" });
        _simpleSearch.Search("STAR", dto).Returns(new SortedSet<string> { "doc1", "doc2", "doc3" });

        var result = _phraseSearch.Search(input, dto);


        result.Should().BeEquivalentTo(new[] { "doc1", "doc2" });
    }

    [Fact]
    public void Search_ShouldReturnEmpty_WhenNoCommonDocumentsAcrossWords()
    {

        var input = "code star";
        _tokenizer.Tokenize(input).Returns(new List<string> { "CODE", "STAR" });

        var dto = CreateDto();

        _simpleSearch.Search("CODE", dto).Returns(new SortedSet<string> { "doc1" });
        _simpleSearch.Search("STAR", dto).Returns(new SortedSet<string> { "doc2" });


        var result = _phraseSearch.Search(input, dto);


        result.Should().BeEmpty();
    }

    [Fact]
    public void Search_ShouldReturnEmpty_WhenAnyWordIsMissingFromIndex()
    {

        var input = "code star";
        _tokenizer.Tokenize(input).Returns(new List<string> { "CODE", "STAR" });

        var dto = CreateDto();

        _simpleSearch.Search("CODE", dto).Returns(new SortedSet<string> { "doc1" });
        _simpleSearch.Search("STAR", dto).Returns(new SortedSet<string>());


        var result = _phraseSearch.Search(input, dto);


        result.Should().BeEmpty();
    }

    private static InvertedIndexDto CreateDto() =>
        new InvertedIndexDto
        {
            AllDocuments = new SortedSet<string> { "doc1", "doc2", "doc3" },
            InvertedIndexMap = new SortedDictionary<string, SortedSet<DocumentInfo>>()
        };
}
