using FullTextSearch.API.InvertedIndex.Dtos;
using FullTextSearch.API.InvertedIndex.FilterStrategies.Abstractions;
using FullTextSearch.API.InvertedIndex.SearchFeatures.Abstractions;

namespace FullTextSearch.API.InvertedIndex.SearchFeatures;

public class AdvancedSearch : IAdvanceSearch
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