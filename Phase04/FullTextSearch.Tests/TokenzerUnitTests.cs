using FluentAssertions;
using FullTextSearch.Services.TokenizerService;

namespace FullTextSearch.Tests;
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
        var expected = _sut.Tokenize("GET -hELp +illNeSS");

        expected.Should()
            .BeEquivalentTo(new List<string> { "GET", "HELP", "ILLNESS" });
    }

    [Fact]
    public void Tokenize_ShouldIgnorePunctuation()
    {
        var expected = _sut.Tokenize("hello, world!");

        expected.Should()
            .BeEquivalentTo(new List<string> { "HELLO", "WORLD" });
    }

    [Fact]
    public void Tokenize_ShouldHandleApostrophes()
    {
        var expected = _sut.Tokenize("it's raining");

        expected.Should()
            .BeEquivalentTo(new List<string> { "IT", "S", "RAINING" });
    }

    [Theory]
    [InlineData(" ")]
    [InlineData("    ")]
    public void Tokenize_ShouldReturnEmpty_ForEmptyString(string queryInput)
    {
        var expected = _sut.Tokenize(queryInput);

        expected.Should()
            .BeEmpty();
    }

    [Fact]
    public void Tokenize_ShouldHandleMixedContent()
    {
        var expected = _sut.Tokenize("Hello123 _test_!");

        expected.Should()
            .BeEquivalentTo(new List<string> { "HELLO123", "_TEST_" });
    }

    [Fact]
    public void Tokenize_ShouldNotReturnInvalidTokens()
    {
        var expected = _sut.Tokenize("   ,,,   !");

        expected.Should()
            .BeEmpty();
    }
}