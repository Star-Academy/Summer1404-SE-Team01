namespace FullTextSearch.InvertedIndexDs.SearchFeatures;

public class InvertedIndexSimpleSearch : ISearch
{
    private readonly InvertedIndex _invertedIndex;

    public InvertedIndexSimpleSearch(InvertedIndex invertedIndex)
    {
        _invertedIndex = invertedIndex;
    }

    public SortedSet<string> Search(string input)
    {
        string upperWord = input.ToUpper();
        _invertedIndex.InvertedIndexMap.TryGetValue(upperWord, out SortedSet<string>? result);
        return result ?? new SortedSet<string>();
    }
}