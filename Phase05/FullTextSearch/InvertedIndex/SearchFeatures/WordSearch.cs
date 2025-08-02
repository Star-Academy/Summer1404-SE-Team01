using FullTextSearch.InvertedIndex.Dtos;
using FullTextSearch.InvertedIndex.SearchFeatures.Abstractions;

namespace FullTextSearch.InvertedIndex.SearchFeatures;

public class WordSearch : ISearch
{
    public SortedSet<string> Search(string input, InvertedIndexDto invIdxDto)
    {
        var key = input.ToUpper();
        return invIdxDto.InvertedIndexMap.TryGetValue(key, out var docs) && docs.Count > 0
            ? new SortedSet<string>(docs.Select(d => d.DocId))
            : new SortedSet<string>();
    }
}