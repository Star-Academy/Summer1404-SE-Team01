
using FullTextSearch.InvertedIndex.QueryBuilder;
using FullTextSearch.InvertedIndex.SearchFeatures;

namespace FullTextSearch.InvertedIndex.FilterSpecifications;

public class NecessarySpecification : ISpecification
{
    public List<string> Keywords { get; }
    private readonly ISearch _simpleSearch;
    private readonly IQueryExtractor _queryExtractor;

    public NecessarySpecification(ISearch simpleSearch, IQueryExtractor queryExtractor, string query)
    {
        _simpleSearch = simpleSearch ?? throw new ArgumentNullException(nameof(simpleSearch));
        _queryExtractor = queryExtractor ?? throw new ArgumentNullException(nameof(queryExtractor));
        Keywords = _queryExtractor.ExtractQueries(query, @"^[^-+]\w+");
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
