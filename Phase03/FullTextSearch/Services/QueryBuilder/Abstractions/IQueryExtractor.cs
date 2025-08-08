namespace FullTextSearch.Services.QueryBuilder.Abstractions;

public interface IQueryExtractor
{
    IReadOnlyCollection<string> ExtractQueries(string query, string pattern);
}
