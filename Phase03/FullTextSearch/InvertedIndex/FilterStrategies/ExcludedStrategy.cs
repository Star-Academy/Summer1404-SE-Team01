using FullTextSearch.InvertedIndex.Dtos;
using FullTextSearch.InvertedIndex.FilterStrategies.Abstractions;
using FullTextSearch.InvertedIndex.QueryBuilder.Abstractions;
using FullTextSearch.InvertedIndex.SearchFeatures.Abstractions;

namespace FullTextSearch.InvertedIndex.FilterStrategies;

public class ExcludedStrategy : IStrategy
{

    private readonly ISearch _simpleSearch;
    private readonly IQueryExtractor _queryExtractor;

    public ExcludedStrategy(ISearch simpleSearch, IQueryExtractor queryExtractor)
    {
        _simpleSearch = simpleSearch ?? throw new ArgumentNullException(nameof(simpleSearch));
        _queryExtractor = queryExtractor ?? throw new ArgumentNullException(nameof(queryExtractor));
    }
    public void FilterDocumentsByQuery(SortedSet<string> result, string query, InvertedIndexDto dto)
    {
        var keywords = _queryExtractor.ExtractQueries(query, @"^\-\w+");

        if (keywords.Count == 0)
        {
            return;
        }

        foreach (var word in keywords)
        {
            var DocsWithoutWord = _simpleSearch.Search(word, dto);
            result.Except(DocsWithoutWord);
        }
    }
}
