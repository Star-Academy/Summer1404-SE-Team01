using FullTextSearch.API.InvertedIndex.Dtos;

namespace FullTextSearch.API.InvertedIndex.SearchFeatures.Abstractions;

public interface ISequentialPhraseFinder
{
    public HashSet<string> FindSequentialPhrase(IList<string> words, HashSet<string> docIdsContainingWords, InvertedIndexDto invIdxDto);
}
