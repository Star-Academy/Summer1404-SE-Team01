using FullTextSearch.InvertedIndex.FilterSpecifications;
using FullTextSearch.InvertedIndex.QueryBuilder;

namespace FullTextSearch.InvertedIndex.SearchFeatures;

public class InvertedIndexAdvancedSearch : ISearch
{
    private readonly InvertedIndex _invertedIndex;
    private readonly IQueryExtractor _queryExtractor;
    private readonly ISpecification _necessarySpecification;
    private readonly ISpecification _optionalSpecification;
    private readonly ISpecification _excludedSpecification;

    public InvertedIndexAdvancedSearch(
        InvertedIndex invertedIndex,
        IQueryExtractor queryExtractor,
        ISearch simpleSearch,
        ISpecification necessarySpecification,
        ISpecification optionalSpecification,
        ISpecification excludedSpecification)
    {
        _invertedIndex = invertedIndex;
        _queryExtractor = queryExtractor;
        _necessarySpecification = necessarySpecification;
        _optionalSpecification = optionalSpecification;
        _excludedSpecification = excludedSpecification;
    }

    public SortedSet<string> Search(string query)
    {
        var searchQuery = _queryExtractor.ExtractQueries(query);
        var result = new SortedSet<string>(_invertedIndex.AllDocuments);

        if (searchQuery.NecessaryWords.Count > 0)
        {
            _necessarySpecification.FilterDocumentsByQuery(result, searchQuery.NecessaryWords);
        }

        if (searchQuery.OptionalWords.Count > 0)
        {
            _optionalSpecification.FilterDocumentsByQuery(result, searchQuery.OptionalWords);
        }

        if (searchQuery.ExcludedWords.Count > 0)
        {
            _excludedSpecification.FilterDocumentsByQuery(result, searchQuery.ExcludedWords);
        }

        return result;
    }
}