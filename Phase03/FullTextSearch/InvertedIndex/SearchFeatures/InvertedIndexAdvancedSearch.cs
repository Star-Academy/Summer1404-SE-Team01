using FullTextSearch.InvertedIndex.FilterSpecifications;

namespace FullTextSearch.InvertedIndex.SearchFeatures;

public class InvertedIndexAdvancedSearch : ISearch
{
    private readonly IInvertedIndexBuilder _invertedIndex;
    private readonly List<ISpecification> _specifications;

    public InvertedIndexAdvancedSearch(
        IInvertedIndexBuilder invertedIndex,
        List<ISpecification> specifications)
    {
        _invertedIndex = invertedIndex;
        _specifications = specifications;

    }

    public SortedSet<string> Search(string query)
    {
        var result = new SortedSet<string>(_invertedIndex.AllDocuments);

        foreach (var specification in _specifications)
        {
            if (specification.Keywords.Count > 0)
            {
                specification.FilterDocumentsByQuery(result);
            }
        }

        return result;
    }
}