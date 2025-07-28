namespace FullTextSearch.InvertedIndex.QueryBuilder.Abstractions;

public interface IQueryExtractor
{
    List<string> ExtractQueries(string query, string pattern);
}
