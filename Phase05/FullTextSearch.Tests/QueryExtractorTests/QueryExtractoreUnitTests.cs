using FluentAssertions;
using FullTextSearch.InvertedIndex.QueryBuilder;

namespace FullTextSearch.Tests.QueryExtractorTests;

public class SingleWordQueryExtractorTests
{
    private readonly SingleWordQueryExtractor _extractor;
    private readonly string _query;
    public SingleWordQueryExtractorTests()
    {
        _extractor = new SingleWordQueryExtractor();
        _query = @"apple -banana +carrot delta -omega +date ""I have orange"" +""hello world"" -""hi man""";
    }

    [Fact]
    public void ExtractQueries_ShouldReturnWords_WithMinusPrefix()
    {
        var pattern = @"\-\w+";

        var result = _extractor.ExtractQueries(_query, pattern);

        result.Should().BeEquivalentTo(new List<string> { "BANANA", "OMEGA" });
    }

    [Fact]
    public void ExtractQueries_ShouldReturnWords_WithPlusPrefix()
    {
        var pattern = @"\+\w+";

        var result = _extractor.ExtractQueries(_query, pattern);

        result.Should().BeEquivalentTo(new List<string> { "CARROT", "DATE" });
    }

    [Fact]
    public void ExtractQueries_ShouldReturnWords_WithoutPrefix()
    {
        var pattern = @"^[^-+""][a-zA-Z]+$";

        var result = _extractor.ExtractQueries(_query, pattern);

        result.Should().BeEquivalentTo(new List<string> { "APPLE", "DELTA" });
    }

    [Fact]
    public void ExtractQueries_ShouldReturnEmpty_ForNoMatches()
    {
        var query = @"apple banana carrot ""we are code star""";
        var pattern = @"\+\w+";

        var result = _extractor.ExtractQueries(query, pattern);

        result.Should().BeEmpty();
    }

    [Fact]
    public void ExtractQueries_ShouldHandleEmptyQuery()
    {
        var query = "";
        var pattern = @"\-\w+";

        var result = _extractor.ExtractQueries(query, pattern);

        result.Should().BeEmpty();
    }

    [Fact]
    public void ExtractQueries_ShouldNotContainsUnspecifiedQueries()
    {
        var query = "90832490 *word &%word $fake -plush +day should";
        var pattern = @"^[-+]?[a-zA-Z]+$";

        var result = _extractor.ExtractQueries(query, pattern);

        result.Should().BeEquivalentTo(new List<string> { "PLUSH", "DAY", "SHOULD" });
    }
}