using FullTextSearch.InvertedIndex.Dtos;
using FullTextSearch.InvertedIndex.FilterStrategies.Abstractions;
using FullTextSearch.InvertedIndex.SearchFeatures.Abstractions;

namespace FullTextSearch.InvertedIndex.FilterStrategies;

public class RequiredStrategy : IFilterStrategy
{
    private readonly ISearch _search;

    public RequiredStrategy(ISearch searchService)
    {
        _search = searchService ?? throw new ArgumentNullException(nameof(searchService));
    }

    public HashSet<string> FilterDocumentsByQuery(QueryDto queryDto, InvertedIndexDto invIndexDto)
    {
        var result = new HashSet<string>(invIndexDto.AllDocuments);

        foreach (var word in queryDto.Required)
        {
            var currentDocIds = _search.Search(word, invIndexDto);

            result.IntersectWith(currentDocIds);
        }

        return result;
    }
}