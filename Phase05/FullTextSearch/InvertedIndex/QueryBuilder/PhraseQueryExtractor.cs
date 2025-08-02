using FullTextSearch.InvertedIndex.QueryBuilder.Abstractions;
using System.Text.RegularExpressions;

namespace FullTextSearch.InvertedIndex.QueryBuilder;

public class PhraseQueryExtractor : IQueryExtractor
{
    public IReadOnlyCollection<string> ExtractQueries(string query, string pattern)
    {
        var matches = Regex.Matches(query, pattern);
        if (matches.Count == 0) return new List<string>();
        return matches.Select(match => match.Groups[1].Value.ToUpper()).ToList();
    }
}