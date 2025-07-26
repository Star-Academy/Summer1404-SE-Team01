using FullTextSearch.InvertedIndexDs.QueryBuilder;
using FullTextSearch.InvertedIndexDs.SearchFeatures;

namespace FullTextSearch.InvertedIndexDs.FilterSpecifications;

public class OptionalSpecification : ISpecification
{
    public List<string> Keywords { get; }
    private readonly ISearch _simpleSearch;
    private readonly IQueryExtractor _queryExtractor;

    public OptionalSpecification(ISearch simpleSearch, IQueryExtractor queryExtractor, string query)
    {
        _simpleSearch = simpleSearch ?? throw new ArgumentNullException(nameof(simpleSearch));
        _queryExtractor = queryExtractor ?? throw new ArgumentNullException(nameof(queryExtractor));
        Keywords = _queryExtractor.ExtractQueries(query, @"^\+\w+");
    }

    public void FilterDocumentsByQuery(SortedSet<string> documents)
    {
        var optionalDocIds = new SortedSet<string>();
        foreach (var word in Keywords)
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
