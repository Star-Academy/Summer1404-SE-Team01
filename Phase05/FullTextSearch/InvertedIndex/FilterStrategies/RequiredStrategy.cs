using FullTextSearch.InvertedIndex.Dtos;
using FullTextSearch.InvertedIndex.FilterStrategies.Abstractions;
using FullTextSearch.InvertedIndex.QueryBuilder.Abstractions;
using FullTextSearch.InvertedIndex.SearchFeatures.Abstractions;

namespace FullTextSearch.InvertedIndex.FilterStrategies;

public class RequiredStrategy : IStrategy
{
    private readonly ISearch _search;
    private readonly IQueryExtractor _queryExtractor;
    private readonly string _pattern;

    public RequiredStrategy(ISearch simpleSearch, IQueryExtractor queryExtractor, string pattern)
    {
        _search = simpleSearch ?? throw new ArgumentNullException(nameof(simpleSearch));
        _queryExtractor = queryExtractor ?? throw new ArgumentNullException(nameof(queryExtractor));
        _pattern = pattern;
    }

    public void FilterDocumentsByQuery(SortedSet<string> result, string query, InvertedIndexDto inIndexDto)
    {
        var keywords = _queryExtractor.ExtractQueries(query, _pattern);

        if (keywords.Count == 0)
        {
            return;
        }

        foreach (var word in keywords)
        {
            var currentDocIds = _search.Search(word, inIndexDto);

            result.IntersectWith(currentDocIds);
        }
    }
}
