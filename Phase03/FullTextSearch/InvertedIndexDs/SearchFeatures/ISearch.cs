namespace FullTextSearch.InvertedIndexDs.SearchFeatures;

public interface ISearch
{
    SortedSet<string> Search(string input);
}