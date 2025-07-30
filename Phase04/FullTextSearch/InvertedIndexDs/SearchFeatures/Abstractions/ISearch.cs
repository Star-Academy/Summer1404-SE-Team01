using FullTextSearch.InvertedIndexDs.Dtos;

namespace FullTextSearch.InvertedIndexDs.SearchFeatures.Abstractions;

public interface ISearch
{
    SortedSet<string> Search(string input, InvertedIndexDto dto);
}