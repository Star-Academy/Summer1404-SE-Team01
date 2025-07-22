namespace FullTextSearch.InvertedIndex.QueryBuilder;

public interface IQueryExtractor
{
    List<string> ExtractQueries(string query, string pattern);
}
