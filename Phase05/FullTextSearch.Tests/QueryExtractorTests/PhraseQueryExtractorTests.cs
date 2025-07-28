using FluentAssertions;
using FullTextSearch.InvertedIndex.QueryBuilder;

namespace FullTextSearch.Tests.QueryExtractorTests;

public class PhraseQueryExtractorTests
{
    private readonly PhraseQueryExtractor _phraseExtractor = new();

    [Fact]
    public void ExtractQueries_ShouldReturnQuotedPhrases()
    {

        string query = @"get +illness +disease -cough -""star academy"" -""academy star"" +""fake phrase""";
        string pattern = @"-""([^""]+)""";

        var result = _phraseExtractor.ExtractQueries(query, pattern);

        result.Should().HaveCount(2)
            .And.BeEquivalentTo(["star academy".ToUpper(), "academy star".ToUpper()]);
    }
    
}