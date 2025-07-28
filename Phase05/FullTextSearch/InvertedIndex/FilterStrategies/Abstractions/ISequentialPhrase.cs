using FullTextSearch.InvertedIndex.Dtos;

namespace FullTextSearch.InvertedIndex.FilterStrategies.Abstractions;

public interface ISequentialPhrase
{
    void FindConsecutivePhrases(SortedSet<string> docIds, IEnumerable<string> phrase, InvertedIndexDto dto);
}