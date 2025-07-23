namespace FullTextSearch.InvertedIndex.FilterSpecifications;

public interface ISpecification
{
    public List<string> Keywords { get; }
    void FilterDocumentsByQuery(SortedSet<string> documents, List<string> words);
}
