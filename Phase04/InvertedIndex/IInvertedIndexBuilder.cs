namespace FullTextSearch.InvertedIndex;

public interface IInvertedIndexBuilder
{
    SortedSet<string> AllDocuments { get; }
    SortedDictionary<string, SortedSet<string>> InvertedIndexMap { get; }
    void Build(Dictionary<string, string> documents);

    IEnumerable<string> SearchWord(string word);
    IEnumerable<string> AdvancedSearch(string query);
}