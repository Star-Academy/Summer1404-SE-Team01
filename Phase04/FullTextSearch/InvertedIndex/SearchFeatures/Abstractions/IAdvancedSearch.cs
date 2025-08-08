using FullTextSearch.InvertedIndex.Dtos;
using FullTextSearch.InvertedIndex.FilterStrategies.Abstractions;

namespace FullTextSearch.InvertedIndex.SearchFeatures.Abstractions;

public interface IAdvancedSearch
{
    HashSet<string> Search(QueryDto queryDto, InvertedIndexDto invIdxDto, IEnumerable<IFilterStrategy> filterStrategies);
}