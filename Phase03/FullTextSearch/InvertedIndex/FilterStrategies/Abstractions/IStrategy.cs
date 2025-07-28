using FullTextSearch.InvertedIndex.Dtos;

namespace FullTextSearch.InvertedIndex.FilterStrategies.Abstractions;

public interface IStrategy
{
    void FilterDocumentsByQuery(SortedSet<string> result, string query, InvertedIndexDto dto);
}
