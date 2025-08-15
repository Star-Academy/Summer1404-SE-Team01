using FullTextSearch.InvertedIndex.Dtos;

namespace FullTextSearch.InvertedIndex.SearchFeatures.Abstractions;

public interface ISequentialValidator
{
    public HashSet<string> Validate(IList<string> words, HashSet<string> docIdsContainingWords, InvertedIndexDto invIdxDto);
}
