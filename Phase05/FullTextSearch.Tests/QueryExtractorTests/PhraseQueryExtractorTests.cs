using FluentAssertions;
using FullTextSearch.Services.QueryBuilder;

namespace FullTextSearch.Tests.QueryExtractorTests;

public class PhraseQueryExtractorTests
{
    private readonly PhraseQueryExtractor _sut;
    public PhraseQueryExtractorTests()
    {
        _sut = new PhraseQueryExtractor();

    }

    [Fact]
    public void ExtractQueries_ShouldReturnQuotedPhrases_WithMinusPrefix()
    {

        string query = @"get +illness +disease -cough -""we are code star"" -""star academy"" -""academy star"" +""fake phrase"" ""no prefix""";
        string pattern = @"-""([^""]+)""";

        var expected = _sut.ExtractQueries(query, pattern);

        expected.Should().HaveCount(3)
            .And.BeEquivalentTo(["star academy".ToUpper(), "academy star".ToUpper(), "we are code star".ToUpper()]);
    }

    [Fact]
    public void ExtractQueries_ShouldReturnQuotedPhrases_WithPlusPrefix()
    {

        string query = @"get +illness +disease +""we are code star"" -cough -""star academy"" -""academy star"" +""fake phrase"" +""plus query"" ""no prefix""";
        string pattern = @"\+""([^""]+)""";

        var expected = _sut.ExtractQueries(query, pattern);

        expected.Should().HaveCount(3)
            .And.BeEquivalentTo(["fake phrase".ToUpper(), "plus query".ToUpper(), "we are code star".ToUpper()]);
    }

    [Fact]
    public void ExtractQueries_ShouldReturnQuotedPhrases_WithoutPrefix()
    {
        string query = @"get +illness +disease -cough ""we are code star"" ""star academy"" -""academy star"" +""plus query"" ""hello world""";
        string pattern = @"(?<!\S)""([^""]+)""";


        var expected = _sut.ExtractQueries(query, pattern);

        expected.Should().HaveCount(3)
            .And.BeEquivalentTo(["star academy".ToUpper(), "hello world".ToUpper(), "we are code star".ToUpper()]);
    }

}