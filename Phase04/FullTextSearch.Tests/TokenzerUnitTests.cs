
using FluentAssertions;
using FullTextSearch.Services.TokenizerService;
using NSubstitute;

namespace FullTextSearch.Tests;
public class TokenizerUnitTests
{
    private readonly Tokenizer _tokenizer;

    public TokenizerUnitTests()
    {
         _tokenizer= new();    
    }
    
    [Fact]
    public void Tokenize_ShouldReturnUppercasedTokens_FromInputQuery()
    {
        var result = _tokenizer.Tokenize("GET -hELp +illNeSS");
        
        result.Should().BeEquivalentTo(new List<string> {"GET", "HELP", "ILLNESS" });
    }

    [Fact]
    public void Tokenize_ShouldIgnorePunctuation()
    {
        var result = _tokenizer.Tokenize("hello, world!");
        Assert.Equal(new List<string> { "HELLO", "WORLD" }, result);
    }

    [Fact]
    public void Tokenize_ShouldHandleApostrophes()
    {
        var result = _tokenizer.Tokenize("it's raining");
        Assert.Equal(new List<string> { "IT", "S", "RAINING" }, result);
    }

    [Theory]
    [InlineData(" ")]
    [InlineData("    ")]
    public void Tokenize_ShouldReturnEmpty_ForEmptyString(string queryInput)
    {
        var result = _tokenizer.Tokenize(queryInput);
        Assert.Empty(result);
    }

    [Fact]
    public void Tokenize_ShouldHandleMixedContent()
    {
        var result = _tokenizer.Tokenize("Hello123 _test_!");
        Assert.Equal(new List<string> { "HELLO123", "_TEST_" }, result);
    }

    [Fact]
    public void Tokenize_ShouldNotReturnInvalidTokens()
    {
        var result = _tokenizer.Tokenize("   ,,,   !");
        Assert.Empty(result);
    }
}