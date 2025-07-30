using FullTextSearch.InvertedIndex.Dtos;
using FullTextSearch.InvertedIndex.SearchFeatures.Abstractions;

namespace FullTextSearch.InvertedIndex.SearchFeatures;

public class SimpleSearch : ISearch
{
    public SortedSet<string> Search(string input, InvertedIndexDto dto)
    {
        var key = input.ToUpper();
        return dto.InvertedIndexMap.TryGetValue(key, out var docs) && docs.Count > 0
            ? new SortedSet<string>(docs.Select(d => d.DocId))
            : new SortedSet<string>();
    }
}