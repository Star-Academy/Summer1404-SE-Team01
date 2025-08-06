using FullTextSearch.API.InvertedIndex.Dtos;
using FullTextSearch.API.InvertedIndex.FilterStrategies.Abstractions;

namespace FullTextSearch.API.InvertedIndex.SearchFeatures.Abstractions;

public interface IAdvanceSearch
{
    HashSet<string> Search(QueryDto queryDto, InvertedIndexDto invIdxDto, List<IFilterStrategy> filterStrategies);
}