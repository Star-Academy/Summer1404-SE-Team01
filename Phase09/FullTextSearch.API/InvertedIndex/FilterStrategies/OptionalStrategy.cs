using FullTextSearch.API.InvertedIndex.Dtos;
using FullTextSearch.API.InvertedIndex.FilterStrategies.Abstractions;
using FullTextSearch.API.InvertedIndex.SearchFeatures.Abstractions;

namespace FullTextSearch.API.InvertedIndex.FilterStrategies;

public class OptionalStrategy : IFilterStrategy
{

    private readonly ISearch _search;
    public OptionalStrategy(ISearch searchService)
    {
        _search = searchService ?? throw new ArgumentNullException(nameof(searchService));
    }

    public HashSet<string> FilterDocumentsByQuery(QueryDto queryDto, InvertedIndexDto invIndexDto)
    {
        var result = new HashSet<string>(invIndexDto.AllDocuments);

        var optionalDocIds = new HashSet<string>();
        foreach (var word in queryDto.Optional)
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
