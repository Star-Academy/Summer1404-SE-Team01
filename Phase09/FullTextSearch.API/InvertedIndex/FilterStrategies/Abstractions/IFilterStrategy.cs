using FullTextSearch.API.InvertedIndex.Dtos;

namespace FullTextSearch.API.InvertedIndex.FilterStrategies.Abstractions;

public interface IFilterStrategy
{
    SortedSet<string> FilterDocumentsByQuery(string input, InvertedIndexDto invIndexDto);
}
