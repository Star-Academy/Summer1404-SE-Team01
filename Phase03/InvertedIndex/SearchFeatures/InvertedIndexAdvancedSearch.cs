using FullTextSearch.InvertedIndex.FilterSpecifications;
using FullTextSearch.InvertedIndex.QueryBuilder;

namespace FullTextSearch.InvertedIndex.SearchFeatures;

public class InvertedIndexAdvancedSearch : ISearch
{
    private readonly IInvertedIndexBuilder _invertedIndex;
    private readonly IQueryExtractor _queryExtractor;
    private readonly List<ISpecification> _specifications;

    public InvertedIndexAdvancedSearch(
        IInvertedIndexBuilder invertedIndex,
        IQueryExtractor queryExtractor,
        List<ISpecification> specifications)
    {
        _invertedIndex = invertedIndex;
        _queryExtractor = queryExtractor;
        _specifications = specifications;

    }

    public SortedSet<string> Search(string query)
    {
        var result = new SortedSet<string>(_invertedIndex.AllDocuments);

        foreach (var specification in _specifications)
        {
            if (specification.Keywords.Count > 0)
            {
                specification.FilterDocumentsByQuery(result, specification.Keywords);
            }
        }

        return result;
    }
}