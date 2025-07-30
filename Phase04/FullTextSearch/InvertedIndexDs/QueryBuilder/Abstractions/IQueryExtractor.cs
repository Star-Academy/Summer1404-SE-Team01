namespace FullTextSearch.InvertedIndexDs.QueryBuilder.Abstractions;

public interface IQueryExtractor
{
    List<string> ExtractQueries(string query, string pattern);
}
