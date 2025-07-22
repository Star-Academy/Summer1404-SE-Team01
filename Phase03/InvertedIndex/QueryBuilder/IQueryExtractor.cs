namespace FullTextSearch.InvertedIndex.QueryBuilder;

public interface IQueryExtractor
{
    SearchQuery ExtractQueries(string query);
}
