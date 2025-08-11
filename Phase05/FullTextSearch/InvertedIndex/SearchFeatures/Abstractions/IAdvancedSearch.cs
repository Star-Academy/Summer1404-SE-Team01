using FullTextSearch.InvertedIndex.Dtos;

namespace FullTextSearch.InvertedIndex.SearchFeatures.Abstractions;

public interface IAdvancedSearch
{
    HashSet<string> Search(QueryDto queryDto, InvertedIndexDto invIdxDto);
}