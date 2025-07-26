using FullTextSearch.InvertedIndexDs.QueryBuilder;
using FullTextSearch.InvertedIndexDs.SearchFeatures;

namespace FullTextSearch.InvertedIndexDs.FilterSpecifications;

public class ExcludedSpecification : ISpecification
{
    public List<string> Keywords { get; }
    private readonly ISearch _simpleSearch;
    private readonly IQueryExtractor _queryExtractor;

    public ExcludedSpecification(ISearch simpleSearch, IQueryExtractor queryExtractor, string query)
    {
        _simpleSearch = simpleSearch ?? throw new ArgumentNullException(nameof(simpleSearch));
        _queryExtractor = queryExtractor ?? throw new ArgumentNullException(nameof(queryExtractor));
        Keywords = _queryExtractor.ExtractQueries(query, @"^\-\w+");
    }

    public void FilterDocumentsByQuery(SortedSet<string> documents)
    {
        foreach (var word in Keywords)
        {
            var currentDocIds = _simpleSearch.Search(word);
            documents.ExceptWith(currentDocIds);
        }
    }
}
