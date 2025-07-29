using FluentAssertions;
using FullTextSearch.InvertedIndex.QueryBuilder;

namespace FullTextSearch.Tests.QueryExtractorTests;

public class QueryExtractorTests
{
    private readonly QueryExtractor _extractor;
    private readonly string _query;
    public QueryExtractorTests()
    {
        _extractor = new QueryExtractor();
        _query = @"apple -banana +carrot delta +date -echo ""I have"" +""hello world"" -""hi man""";
    }

    [Fact]
    public void ExtractQueries_ShouldReturnWords_WithMinusPrefix()
    {
        var pattern = @"\-\w+";

        var result = _extractor.ExtractQueries(_query, pattern);

        result.Should().BeEquivalentTo(new List<string> { "BANANA", "ECHO" });
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
        var query = @"apple -banana +carrot delta echo ""I have"" +""hello world"" -""hi man""";
        var pattern = @"^[^-+""][a-zA-Z]+$";

        var result = _extractor.ExtractQueries(query, pattern);

        result.Should().BeEquivalentTo(new List<string> { "APPLE", "DELTA", "ECHO" });
    }

    [Fact]
    public void ExtractQueries_ShouldReturnEmpty_ForNoMatches()
    {
        var query = "apple banana carrot";
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

    //[Fact]
    //public void ExtractQueries_ShouldNotContainsUnspecifiedQueries()
    //{
    //    var query = "90832490 *word &%word $fake -plush +day should";
    //    var pattern = @"[-+][a-zA-Z]+";

    //    var result = _extractor.ExtractQueries(query, pattern);

    //    result.Should().BeEquivalentTo(new List<string> { "PLUSH", "DAY", "SHOULD" });
    //}
}