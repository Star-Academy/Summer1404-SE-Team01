
using FullTextSearch.InvertedIndex.SearchFeatures;

namespace FullTextSearch.InvertedIndex.FilterSpecifications;

public class OptionalSpecification : ISpecification
{
    private readonly ISearch _simpleSearch;

    public OptionalSpecification(ISearch simpleSearch)
    {
        _simpleSearch = simpleSearch;
    }

    public void FilterDocumentsByQuery(SortedSet<string> documents, List<string> words)
    {
        var optionalDocIds = new SortedSet<string>();
        foreach (var word in words)
        {
            var currentDocIds = _simpleSearch.Search(word);
            optionalDocIds.UnionWith(currentDocIds);
        }

        if (optionalDocIds.Count != 0)
        {
            documents.IntersectWith(optionalDocIds);
        }
    }
}
