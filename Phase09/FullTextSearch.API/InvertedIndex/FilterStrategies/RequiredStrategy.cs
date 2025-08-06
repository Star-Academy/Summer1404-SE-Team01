using FullTextSearch.API.InvertedIndex.Dtos;
using FullTextSearch.API.InvertedIndex.FilterStrategies.Abstractions;
using FullTextSearch.API.InvertedIndex.SearchFeatures.Abstractions;

namespace FullTextSearch.API.InvertedIndex.FilterStrategies;

public class RequiredStrategy : IFilterStrategy
{
    private readonly ISearch _search;

    public RequiredStrategy(ISearch searchType)
    {
        _search = searchType ?? throw new ArgumentNullException(nameof(searchType));
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
