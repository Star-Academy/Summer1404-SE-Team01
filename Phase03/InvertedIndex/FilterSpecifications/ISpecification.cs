namespace FullTextSearch.InvertedIndex.FilterSpecifications;

public interface ISpecification
{
    void FilterDocumentsByQuery(SortedSet<string> documents, List<string> words);
}
