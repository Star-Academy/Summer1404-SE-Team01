using FullTextSearch.API.InvertedIndex.Dtos;

namespace FullTextSearch.API.InvertedIndex.SearchFeatures.Abstractions;

public interface ISequentialValidator
{
    public SortedSet<string> Validate(IList<string> words, SortedSet<string> docIdsContainingWords, InvertedIndexDto invIdxDto);
}
