using Xunit;
using FluentAssertions;
using System.Collections.Generic;
using FullTextSearch.InvertedIndex.QueryBuilder;

namespace FullTextSearch.Tests;

public class QueryExtractorTests
{
    private readonly QueryExtractor _extractor = new();

    [Fact]
    public void ExtractQueries_ShouldReturnWords_StartingWithMinus()
    {
        var query = "apple -banana -carrot +date";
        var pattern = @"^\-\w+";

        var result = _extractor.ExtractQueries(query, pattern);

        result.Should().BeEquivalentTo(new List<string> { "-BANANA", "-CARROT" });
    }

    [Fact]
    public void ExtractQueries_ShouldReturnWords_NotStartingWithPlusOrMinus()
    {
        var query = "apple -banana +carrot delta echo";
        var pattern = @"^[^-+]\w+";

        var result = _extractor.ExtractQueries(query, pattern);

        result.Should().BeEquivalentTo(new List<string> { "APPLE", "DELTA", "ECHO" });
    }

    [Fact]
    public void ExtractQueries_ShouldReturnWords_StartingWithPlus()
    {
        var query = "apple +banana -carrot +date";
        var pattern = @"^\+\w+";

        var result = _extractor.ExtractQueries(query, pattern);

        result.Should().BeEquivalentTo(new List<string> { "+BANANA", "+DATE" });
    }

    [Fact]
    public void ExtractQueries_ShouldReturnEmpty_ForNoMatches()
    {
        var query = "apple banana carrot";
        var pattern = @"^\+\w+";

        var result = _extractor.ExtractQueries(query, pattern);

        result.Should().BeEmpty();
    }

    [Fact]
    public void ExtractQueries_ShouldHandleEmptyQuery()
    {
        var query = "";
        var pattern = @"^\-\w+";

        var result = _extractor.ExtractQueries(query, pattern);

        result.Should().BeEmpty();
    }
}