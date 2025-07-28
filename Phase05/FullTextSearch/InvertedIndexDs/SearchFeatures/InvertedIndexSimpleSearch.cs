namespace FullTextSearch.InvertedIndexDs.SearchFeatures;

public class InvertedIndexSimpleSearch : ISearch
{
    private readonly InvertedIndex _invertedIndex;

    public InvertedIndexSimpleSearch(InvertedIndex invertedIndex)
        => _invertedIndex = invertedIndex;

    public SortedSet<string> Search(string input)
    {
        var key = input.ToUpper();
        return _invertedIndex.InvertedIndexMap.TryGetValue(key, out var docs) && docs.Count > 0
            ? new SortedSet<string>(docs.Select(d => d.DocId))
            : new SortedSet<string>();
    }
}