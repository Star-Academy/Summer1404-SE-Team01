using System.Text.RegularExpressions;
using FullTextSearch.InvertedIndexDs.QueryBuilder.Abstractions;

namespace FullTextSearch.InvertedIndexDs.QueryBuilder;

public class PhraseQueryExtractor : IQueryExtractor
{
    public List<string> ExtractQueries(string query, string pattern)
    {
        var matches =  Regex.Matches(query, pattern); 
        return matches.Select(match => match.Groups[1].Value).ToList();
    }
}