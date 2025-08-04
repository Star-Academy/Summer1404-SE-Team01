using FluentAssertions;
using FullTextSearch.API.Services.TokenizerService;

namespace FullTextSearch.API.Tests;
public class TokenizerUnitTests
{
    private readonly Tokenizer _sut;

    public TokenizerUnitTests()
    {
        _sut = new Tokenizer();
    }

    [Fact]
    public void Tokenize_ShouldReturnUppercasedTokens_FromInputQuery()
    {
        var result = _sut.Tokenize("GET -hELp +illNeSS");

        result.Should()
            .BeEquivalentTo(new List<string> { "GET", "HELP", "ILLNESS" });
    }

    [Fact]
    public void Tokenize_ShouldIgnorePunctuation()
    {
        var result = _sut.Tokenize("hello, world!");

        result.Should()
            .BeEquivalentTo(new List<string> { "HELLO", "WORLD" });
    }

    [Fact]
    public void Tokenize_ShouldHandleApostrophes()
    {
        var result = _sut.Tokenize("it's raining");

        result.Should()
            .BeEquivalentTo(new List<string> { "IT", "S", "RAINING" });
    }

    [Theory]
    [InlineData(" ")]
    [InlineData("    ")]
    public void Tokenize_ShouldReturnEmpty_ForEmptyString(string queryInput)
    {
        var result = _sut.Tokenize(queryInput);

        result.Should()
            .BeEmpty();
    }

    [Fact]
    public void Tokenize_ShouldHandleMixedContent()
    {
        var result = _sut.Tokenize("Hello123 _test_!");

        result.Should()
            .BeEquivalentTo(new List<string> { "HELLO123", "_TEST_" });
    }

    [Fact]
    public void Tokenize_ShouldNotReturnInvalidTokens()
    {
        var result = _sut.Tokenize("   ,,,   !");

        result.Should()
            .BeEmpty();
    }
}