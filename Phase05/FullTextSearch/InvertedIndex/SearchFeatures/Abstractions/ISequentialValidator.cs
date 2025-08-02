using FullTextSearch.InvertedIndex.Dtos;

namespace FullTextSearch.InvertedIndex.SearchFeatures.Abstractions;

public interface ISequentialValidator
{
    public SortedSet<string> Validate(IList<string> words, SortedSet<string> docIdsContainingWords, InvertedIndexDto invIdxDto);
}
