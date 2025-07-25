namespace FullTextSearch.InvertedIndexDs.QueryBuilder;

public interface IQueryExtractor
{
    List<string> ExtractQueries(string query, string pattern);
}
