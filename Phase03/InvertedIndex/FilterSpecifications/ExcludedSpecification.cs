
using FullTextSearch.InvertedIndex.SearchFeatures;

namespace FullTextSearch.InvertedIndex.FilterSpecifications;

public class ExcludedSpecification : ISpecification
{
    private readonly ISearch _simpleSearch;

    public ExcludedSpecification(ISearch simpleSearch)
    {
        _simpleSearch = simpleSearch;
    }

    public void FilterDocumentsByQuery(SortedSet<string> documents, List<string> words)
    {
        foreach (var word in words)
        {
            var currentDocIds = _simpleSearch.Search(word);
            documents.ExceptWith(currentDocIds);
        }
    }
}
