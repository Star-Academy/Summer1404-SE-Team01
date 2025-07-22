namespace FullTextSearch.InvertedIndex;

public interface IInvertedIndex
{
    public void BuildIndex(Dictionary<string, string> documents);
    SortedSet<string> SearchWord(string word);
    SortedSet<string> AdvancedSearch(string query);
}