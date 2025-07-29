using FullTextSearch.InvertedIndex.Dtos;
using FullTextSearch.InvertedIndex.FilterStrategies.Abstractions;
using FullTextSearch.InvertedIndex.QueryBuilder.Abstractions;
using FullTextSearch.InvertedIndex.SearchFeatures.Abstractions;

namespace FullTextSearch.InvertedIndex.FilterStrategies;

public class OptionalStrategy : IStrategy
{

    private readonly ISearch _simpleSearch;
    private readonly IQueryExtractor _queryExtractor;
    private readonly string _pattern;

    public OptionalStrategy(ISearch simpleSearch, IQueryExtractor queryExtractor, string pattern)
    {
        _simpleSearch = simpleSearch ?? throw new ArgumentNullException(nameof(simpleSearch));
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

        var optionalDocIds = new SortedSet<string>();
        foreach (var word in keywords)
        {
            var currentDocIds = _simpleSearch.Search(word, inIndexDto);
            optionalDocIds.UnionWith(currentDocIds);
        }

        if (optionalDocIds.Count != 0)
        {
            result.IntersectWith(optionalDocIds);
        }

    }
}
