using FullTextSearch.InvertedIndex;
using FluentAssertions;
using NSubstitute;

using FullTextSearch.Services.TokenizerService;

namespace FullTextSearch.Tests;
public class InvertedIndexUnitTests
{
    
    private readonly ITokenizer _tokenizer;
    private readonly IInvertedIndexBuilder _invertedIndexBuilder;

    public InvertedIndexUnitTests()
    {
        _tokenizer = Substitute.For<ITokenizer>();
        _invertedIndexBuilder = new InvertedIndex.InvertedIndex(_tokenizer);
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

        _invertedIndexBuilder.Build(docs);

        var indexMap = _invertedIndexBuilder.InvertedIndexMap;

        indexMap.Should().ContainKey("APPLE").WhoseValue.Should().BeEquivalentTo("doc1", "doc3");
        indexMap.Should().ContainKey("BANANA").WhoseValue.Should().BeEquivalentTo("doc1", "doc2");
        indexMap.Should().ContainKey("CARROT").WhoseValue.Should().BeEquivalentTo("doc2", "doc3");
    }
}