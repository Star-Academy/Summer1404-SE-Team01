using FullTextSearch.API.InvertedIndex.Dtos;
using FullTextSearch.API.InvertedIndex.FilterStrategies.Abstractions;
using FullTextSearch.API.InvertedIndex.SearchFeatures.Abstractions;

namespace FullTextSearch.API.InvertedIndex.SearchFeatures;

public class AdvancedSearch : ISearch
{
    private readonly List<IFilterStrategy> _filterStrategies;

    public AdvancedSearch(List<IFilterStrategy> strategies)
    {
        _filterStrategies = strategies;
    }

    public SortedSet<string> Search(string query, InvertedIndexDto invIdxDto)
    {
        var result = new SortedSet<string>(invIdxDto.AllDocuments);

        foreach (var strategy in _filterStrategies)
        {
            var currDocIds = strategy.FilterDocumentsByQuery(query, invIdxDto);
            result.IntersectWith(currDocIds);
        }

        return result;
    }
}