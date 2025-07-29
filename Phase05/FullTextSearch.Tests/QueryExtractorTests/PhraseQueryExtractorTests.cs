using FluentAssertions;
using FullTextSearch.InvertedIndex.QueryBuilder;

namespace FullTextSearch.Tests.QueryExtractorTests;

public class PhraseQueryExtractorTests
{
    private readonly PhraseQueryExtractor _phraseExtractor;
    public PhraseQueryExtractorTests()
    {
        _phraseExtractor = new PhraseQueryExtractor();

    }

    [Fact]
    public void ExtractQueries_ShouldReturnQuotedPhrases_WithMinusPrefix()
    {

        string query = @"get +illness +disease -cough -""star academy"" -""academy star"" +""fake phrase"" ""no prefix""";
        string pattern = @"-""([^""]+)""";

        var result = _phraseExtractor.ExtractQueries(query, pattern);

        result.Should().HaveCount(2)
            .And.BeEquivalentTo(["star academy".ToUpper(), "academy star".ToUpper()]);
    }

    [Fact]
    public void ExtractQueries_ShouldReturnQuotedPhrases_WithPlusPrefix()
    {

        string query = @"get +illness +disease -cough -""star academy"" -""academy star"" +""fake phrase"" +""plus query"" ""no prefix""";
        string pattern = @"\+""([^""]+)""";

        var result = _phraseExtractor.ExtractQueries(query, pattern);

        result.Should().HaveCount(2)
            .And.BeEquivalentTo(["fake phrase".ToUpper(), "plus query".ToUpper()]);
    }

    [Fact]
    public void ExtractQueries_ShouldReturnQuotedPhrases_WithoutPrefix()
    {
        string query = @"get +illness +disease -cough ""star academy"" -""academy star"" +""plus query"" ""hello world""";
        string pattern = @"(?<!\S)""([^""]+)""";


        var result = _phraseExtractor.ExtractQueries(query, pattern);

        result.Should().HaveCount(2)
            .And.BeEquivalentTo(["star academy".ToUpper(), "hello world".ToUpper()]);
    }

}