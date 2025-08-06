using FullTextSearch.API.InvertedIndex.Dtos;

namespace FullTextSearch.API.InvertedIndex.SearchFeatures.Abstractions;

public interface ISequentialValidator
{
    public HashSet<string> Validate(IList<string> words, HashSet<string> docIdsContainingWords, InvertedIndexDto invIdxDto);
}
