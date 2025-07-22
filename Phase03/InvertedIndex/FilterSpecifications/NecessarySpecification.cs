
using FullTextSearch.InvertedIndex.SearchFeatures;

namespace FullTextSearch.InvertedIndex.FilterSpecifications;

public class NecessarySpecification : ISpecification
{
    private readonly ISearch _simpleSearch;

    public NecessarySpecification(ISearch simpleSearch)
    {
        _simpleSearch = simpleSearch;
    }

    public void FilterDocumentsByQuery(SortedSet<string> documents, List<string> words)
    {
        foreach (var word in words)
        {
            var currentDocIds = _simpleSearch.Search(word);

            documents.IntersectWith(currentDocIds);
        }
    }
}
