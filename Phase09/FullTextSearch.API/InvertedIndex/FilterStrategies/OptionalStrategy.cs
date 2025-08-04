using FullTextSearch.API.InvertedIndex.Dtos;
using FullTextSearch.API.InvertedIndex.FilterStrategies.Abstractions;
using FullTextSearch.API.InvertedIndex.QueryBuilder.Abstractions;
using FullTextSearch.API.InvertedIndex.SearchFeatures.Abstractions;

namespace FullTextSearch.API.InvertedIndex.FilterStrategies;

public class OptionalStrategy : IFilterStrategy
{

    private readonly ISearch _search;
    private readonly IQueryExtractor _queryExtractor;
    private readonly string _pattern;

    public OptionalStrategy(ISearch searchType, IQueryExtractor queryExtractor, string pattern)
    {
        _search = searchType ?? throw new ArgumentNullException(nameof(searchType));
        _queryExtractor = queryExtractor ?? throw new ArgumentNullException(nameof(queryExtractor));
        _pattern = pattern;
    }

    public SortedSet<string> FilterDocumentsByQuery(string query, InvertedIndexDto invIndexDto)
    {
        var keywords = _queryExtractor.ExtractQueries(query, _pattern);
        var result = new SortedSet<string>(invIndexDto.AllDocuments);

        var optionalDocIds = new SortedSet<string>();
        foreach (var word in keywords)
        {
            var currentDocIds = _search.Search(word, invIndexDto);
            optionalDocIds.UnionWith(currentDocIds);
        }

        if (optionalDocIds.Count > 0)
        {
            result.IntersectWith(optionalDocIds);
        }

        return result;

    }
}
