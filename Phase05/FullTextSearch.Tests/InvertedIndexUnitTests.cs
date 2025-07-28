using FluentAssertions;
using FullTextSearch.InvertedIndexDs;
using FullTextSearch.InvertedIndexDs.Dtos;
using FullTextSearch.Services.TokenizerService;
using NSubstitute;

namespace FullTextSearch.Tests;
public class InvertedIndexUnitTests
{

    private readonly ITokenizer _tokenizer;
    private readonly InvertedIndexBuilder _invertedIndexBuilder;

    public InvertedIndexUnitTests()
    {
        _tokenizer = Substitute.For<ITokenizer>();
        _invertedIndexBuilder = new InvertedIndexBuilder(_tokenizer);
    }


    [Fact]
    public void Build_ShouldPopulateInvertedIndexMap_Correctly()
    {
        var docs = new Dictionary<string, string>
        {
            { "doc1", "apple banana" },
            { "doc2", "banana carrot" },
            { "doc3", "apple carrot" }
        };

        _tokenizer.Tokenize("apple banana").Returns(new[] { "APPLE", "BANANA" });
        _tokenizer.Tokenize("banana carrot").Returns(new[] { "BANANA", "CARROT" });
        _tokenizer.Tokenize("apple carrot").Returns(new[] { "APPLE", "CARROT" });

        var dto = _invertedIndexBuilder.Build(docs);

        var expectedAppleValue = new SortedSet<DocumentInfo>
        {
            new DocumentInfo
            {
                DocId = "doc1",
                Indexes = { 0 }
            },
            new DocumentInfo
            {
                DocId = "doc3",
                Indexes = { 0 }
            }
        };
        dto.InvertedIndexMap.Should().ContainKey("APPLE").WhoseValue.Should().BeEquivalentTo(expectedAppleValue);

        var expectedBananaValue = new SortedSet<DocumentInfo>
        {
            new DocumentInfo
            {
                DocId = "doc1",
                Indexes = { 1 }
            },
            new DocumentInfo
            {
                DocId = "doc2",
                Indexes = { 0 }
            }
        };
        dto.InvertedIndexMap.Should().ContainKey("BANANA").WhoseValue.Should().BeEquivalentTo(expectedBananaValue);
        
        var expectedCarrotValue = new SortedSet<DocumentInfo>
        {
            new DocumentInfo
            {
                DocId = "doc2",
                Indexes = { 1 }
            },
            new DocumentInfo
            {
                DocId = "doc3",
                Indexes = { 1 }
            }
        };
        dto.InvertedIndexMap.Should().ContainKey("CARROT").WhoseValue.Should().BeEquivalentTo(expectedCarrotValue);
    }
}