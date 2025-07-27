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
        _invertedIndex.InvertedIndexMap.TryGetValue(upperWord, out SortedSet<DocumentInfo>? result);
        if (result is not null && result.Count == 0) 
            return new SortedSet<string>();
        
        var docIds = result?.Select(d => d.DocId).ToHashSet();
        return new SortedSet<string>(docIds!);
    }
}