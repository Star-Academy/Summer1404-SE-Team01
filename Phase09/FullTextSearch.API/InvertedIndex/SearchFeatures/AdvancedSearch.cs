using FullTextSearch.API.InvertedIndex.Dtos;
using FullTextSearch.API.InvertedIndex.FilterStrategies.Abstractions;
using FullTextSearch.API.InvertedIndex.SearchFeatures.Abstractions;

namespace FullTextSearch.API.InvertedIndex.SearchFeatures;

public class AdvancedSearch : IAdvanceSearch
{
    private readonly IEnumerable<IFilterStrategy> _filterStrategies;

    public AdvancedSearch(IEnumerable<IFilterStrategy> filterStrategies)
    {
        _filterStrategies = filterStrategies ?? throw new ArgumentNullException(nameof(filterStrategies));
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