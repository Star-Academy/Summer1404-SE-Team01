namespace FullTextSearch.InvertedIndex.SearchFeatures;

public interface ISearch
{
    SortedSet<string> Search(string input);
}