using FluentAssertions;
using FullTextSearch.InvertedIndexDs.QueryBuilder;
using Xunit;

namespace FullTextSearch.Tests;

public class PhraseQueryExtractorTests
{
    private readonly PhraseQueryExtractor _phraseExtractor = new();

    [Fact]
    public void ExtractQueries_ShouldReturnQuotedPhrases()
    {
        
        string query = @"get +illness +disease -cough -""star academy"" +""fake phrase""";
        string pattern = @"-""([^""]+)"""; // regex to match phrases inside quotes

        var result = _phraseExtractor.ExtractQueries(query, pattern);

        result.Should().ContainSingle()
            .And.Contain("star academy");
    }
    
    
}