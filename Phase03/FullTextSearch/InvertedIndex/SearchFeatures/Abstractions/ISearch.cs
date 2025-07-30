using FullTextSearch.InvertedIndex.Dtos;

namespace FullTextSearch.InvertedIndex.SearchFeatures.Abstractions;

public interface ISearch
{
    SortedSet<string> Search(string input, InvertedIndexDto dto);
}