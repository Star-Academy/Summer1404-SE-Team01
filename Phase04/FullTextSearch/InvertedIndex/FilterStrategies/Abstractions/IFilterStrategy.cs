using FullTextSearch.InvertedIndex.Dtos;

namespace FullTextSearch.InvertedIndex.FilterStrategies.Abstractions;

public interface IFilterStrategy
{
    HashSet<string> FilterDocumentsByQuery(QueryDto queryDto, InvertedIndexDto invIndexDto);
}