using FullTextSearch.InvertedIndexDs.Dtos;
using FullTextSearch.InvertedIndexDs.FilterSpecifications.Abstractions;
using FullTextSearch.InvertedIndexDs.QueryBuilder.Abstractions;
using FullTextSearch.InvertedIndexDs.SearchFeatures.Abstractions;

namespace FullTextSearch.InvertedIndexDs.FilterSpecifications;

public class NecessarySpecification : ISpecification
{
    private readonly ISearch _simpleSearch;
    private readonly IQueryExtractor _queryExtractor;

    public NecessarySpecification(ISearch simpleSearch, IQueryExtractor queryExtractor)
    {
        _simpleSearch = simpleSearch ?? throw new ArgumentNullException(nameof(simpleSearch));
        _queryExtractor = queryExtractor ?? throw new ArgumentNullException(nameof(queryExtractor));
        
    }

    public void FilterDocumentsByQuery(SortedSet<string> result, string query, InvertedIndexDto dto)
    {
        var keywords = _queryExtractor.ExtractQueries(query, @"^[^-+]\w+");
        foreach (var word in keywords)
        {
            var currentDocIds = _simpleSearch.Search(word, dto);

            result.IntersectWith(currentDocIds);
        }
    }
}
