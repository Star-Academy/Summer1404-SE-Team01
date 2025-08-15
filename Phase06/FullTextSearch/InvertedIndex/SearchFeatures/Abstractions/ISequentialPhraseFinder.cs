using FullTextSearch.InvertedIndex.Dtos;

namespace FullTextSearch.InvertedIndex.SearchFeatures.Abstractions;

public interface ISequentialPhraseFinder
{
    public HashSet<string> FindSequentialPhrase(IList<string> words, HashSet<string> docIdsContainingWords, InvertedIndexDto invIdxDto);
}
