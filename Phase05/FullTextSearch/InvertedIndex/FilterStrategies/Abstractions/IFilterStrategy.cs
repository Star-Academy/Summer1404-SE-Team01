using FullTextSearch.InvertedIndex.Dtos;

namespace FullTextSearch.InvertedIndex.FilterStrategies.Abstractions;

public interface IFilterStrategy
{
    SortedSet<string> FilterDocumentsByQuery(string input, InvertedIndexDto invIndexDto);
}
