using FullTextSearch.InvertedIndex.Dtos;
using FullTextSearch.InvertedIndex.FilterStrategies.Abstractions;
using FullTextSearch.InvertedIndex.SearchFeatures.Abstractions;

namespace FullTextSearch.InvertedIndex.SearchFeatures;

public class AdvancedSearch : IAdvancedSearch
{

    public HashSet<string> Search(QueryDto queryDto, InvertedIndexDto invIdxDto, IEnumerable<IFilterStrategy> filterStrategies)
    {
        var result = new HashSet<string>(invIdxDto.AllDocuments);

        foreach (var strategy in filterStrategies)
        {
            var currDocIds = strategy.FilterDocumentsByQuery(queryDto, invIdxDto);
            result.IntersectWith(currDocIds);
        }

        return result;
    }
}