namespace FullTextSearch.InvertedIndexDs;

public interface IInvertedIndexBuilder
{
    SortedSet<string> AllDocuments { get; }
    SortedDictionary<string, SortedSet<DocumentInfo>> InvertedIndexMap { get; }
    void Build(Dictionary<string, string> documents);

}