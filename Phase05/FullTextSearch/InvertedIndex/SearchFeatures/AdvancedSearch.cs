using FullTextSearch.InvertedIndex.Dtos;
using FullTextSearch.InvertedIndex.FilterStrategies.Abstractions;
using FullTextSearch.InvertedIndex.SearchFeatures.Abstractions;

namespace FullTextSearch.InvertedIndex.SearchFeatures;

public class AdvancedSearch : ISearch
{
    private readonly List<IStrategy> _filterStrategies;

    public AdvancedSearch(List<IStrategy> strategies)
    {
        _filterStrategies = strategies;
    }

    public SortedSet<string> Search(string query, InvertedIndexDto dto)
    {
        var result = new SortedSet<string>(dto.AllDocuments);

        foreach (var strategy in _filterStrategies)
        {

            strategy.FilterDocumentsByQuery(result, query, dto);
        }
        return result;
    }
}