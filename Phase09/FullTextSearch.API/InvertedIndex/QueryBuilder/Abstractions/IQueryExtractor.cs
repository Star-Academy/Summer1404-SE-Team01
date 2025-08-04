namespace FullTextSearch.API.InvertedIndex.QueryBuilder.Abstractions;

public interface IQueryExtractor
{
    IReadOnlyCollection<string> ExtractQueries(string query, string pattern);
}
