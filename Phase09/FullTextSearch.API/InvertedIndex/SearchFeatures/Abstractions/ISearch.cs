using FullTextSearch.API.InvertedIndex.Dtos;

namespace FullTextSearch.API.InvertedIndex.SearchFeatures.Abstractions;

public interface ISearch
{
    HashSet<string> Search(string input, InvertedIndexDto invIdxDto);
}