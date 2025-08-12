using FullTextSearch.InvertedIndex.Dtos;
using FullTextSearch.InvertedIndex.FilterStrategies.Abstractions;
using FullTextSearch.InvertedIndex.SearchFeatures.Abstractions;

namespace FullTextSearch.InvertedIndex.SearchFeatures;

public class AdvancedSearch : IAdvancedSearch
{
    private IEnumerable<IFilterStrategy> _filterStrategies;
    public AdvancedSearch(IEnumerable<IFilterStrategy> filterStrategies)
    {
        _filterStrategies = filterStrategies;
    }

    public HashSet<string> Search(QueryDto queryDto, InvertedIndexDto invIdxDto)
    {
        var result = new HashSet<string>(invIdxDto.AllDocuments);

        foreach (var strategy in _filterStrategies)
        {
            var currDocIds = strategy.FilterDocumentsByQuery(queryDto, invIdxDto);
            result.IntersectWith(currDocIds);
        }

        return result;
    }
}