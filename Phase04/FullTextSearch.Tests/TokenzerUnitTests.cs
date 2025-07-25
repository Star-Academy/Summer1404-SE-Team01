using FluentAssertions;
using FullTextSearch.Services.TokenizerService;

namespace FullTextSearch.Tests;
public class TokenizerUnitTests
{
    private readonly Tokenizer _tokenizer;

    public TokenizerUnitTests()
    {
        _tokenizer = new Tokenizer();
    }

    [Fact]
    public void Tokenize_ShouldReturnUppercasedTokens_FromInputQuery()
    {
        var result = _tokenizer.Tokenize("GET -hELp +illNeSS");

        result.Should()
            .BeEquivalentTo(new List<string> { "GET", "HELP", "ILLNESS" });
    }

    [Fact]
    public void Tokenize_ShouldIgnorePunctuation()
    {
        var result = _tokenizer.Tokenize("hello, world!");

        result.Should()
            .BeEquivalentTo(new List<string> { "HELLO", "WORLD" });
    }

    [Fact]
    public void Tokenize_ShouldHandleApostrophes()
    {
        var result = _tokenizer.Tokenize("it's raining");

        result.Should()
            .BeEquivalentTo(new List<string> { "IT", "S", "RAINING" });
    }

    [Theory]
    [InlineData(" ")]
    [InlineData("    ")]
    public void Tokenize_ShouldReturnEmpty_ForEmptyString(string queryInput)
    {
        var result = _tokenizer.Tokenize(queryInput);

        result.Should()
            .BeEmpty();
    }

    [Fact]
    public void Tokenize_ShouldHandleMixedContent()
    {
        var result = _tokenizer.Tokenize("Hello123 _test_!");

        result.Should()
            .BeEquivalentTo(new List<string> { "HELLO123", "_TEST_" });
    }

    [Fact]
    public void Tokenize_ShouldNotReturnInvalidTokens()
    {
        var result = _tokenizer.Tokenize("   ,,,   !");

        result.Should()
            .BeEmpty();
    }
}