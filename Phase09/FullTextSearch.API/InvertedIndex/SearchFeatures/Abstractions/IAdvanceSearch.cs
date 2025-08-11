using FullTextSearch.API.InvertedIndex.Dtos;

namespace FullTextSearch.API.InvertedIndex.SearchFeatures.Abstractions;

public interface IAdvanceSearch
{
    HashSet<string> Search(QueryDto queryDto, InvertedIndexDto invIdxDto);
}