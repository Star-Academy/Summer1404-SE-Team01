using FullTextSearch.InvertedIndex.Dtos;

namespace FullTextSearch.InvertedIndex.SearchFeatures.Abstractions;

public interface ISearch
{
    HashSet<string> Search(string input, InvertedIndexDto invIdxDto);
}