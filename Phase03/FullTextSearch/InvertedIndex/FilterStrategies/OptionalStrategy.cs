using FullTextSearch.InvertedIndex.Dtos;
using FullTextSearch.InvertedIndex.FilterStrategies.Abstractions;
using FullTextSearch.InvertedIndex.QueryBuilder.Abstractions;
using FullTextSearch.InvertedIndex.SearchFeatures.Abstractions;

namespace FullTextSearch.InvertedIndex.FilterStrategies;

public class OptionalStrategy : IStrategy
{

    private readonly ISearch _simpleSearch;
    private readonly IQueryExtractor _queryExtractor;

    public OptionalStrategy(ISearch simpleSearch, IQueryExtractor queryExtractor)
    {
        _simpleSearch = simpleSearch ?? throw new ArgumentNullException(nameof(simpleSearch));
        _queryExtractor = queryExtractor ?? throw new ArgumentNullException(nameof(queryExtractor));
    }

    public void FilterDocumentsByQuery(SortedSet<string> result, string query, InvertedIndexDto dto)
    {
        var keywords = _queryExtractor.ExtractQueries(query, @"^\+\w+");

        if (keywords.Count == 0)
        {
            return;
        }

        var optionalDocIds = new SortedSet<string>();
        foreach (var word in keywords)
        {
            var currentDocIds = _simpleSearch.Search(word, dto);
            optionalDocIds.UnionWith(currentDocIds);
        }

        if (optionalDocIds.Count != 0)
        {
            result.IntersectWith(optionalDocIds);
        }

    }
}
