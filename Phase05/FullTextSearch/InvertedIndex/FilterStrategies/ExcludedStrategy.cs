using FullTextSearch.InvertedIndex.Dtos;
using FullTextSearch.InvertedIndex.FilterStrategies.Abstractions;
using FullTextSearch.InvertedIndex.QueryBuilder.Abstractions;
using FullTextSearch.InvertedIndex.SearchFeatures.Abstractions;

namespace FullTextSearch.InvertedIndex.FilterStrategies;

public class ExcludedStrategy : IFilterStrategy
{

    private readonly ISearch _search;
    private readonly IQueryExtractor _queryExtractor;
    private readonly string _pattern;

    public ExcludedStrategy(ISearch searchType, IQueryExtractor queryExtractor, string pattern)
    {
        _search = searchType ?? throw new ArgumentNullException(nameof(searchType));
        _queryExtractor = queryExtractor ?? throw new ArgumentNullException(nameof(queryExtractor));
        _pattern = pattern;
    }
    public SortedSet<string> FilterDocumentsByQuery(string query, InvertedIndexDto invIndexDto)
    {
        var keywords = _queryExtractor.ExtractQueries(query, _pattern);
        var result = new SortedSet<string>(invIndexDto.AllDocuments);

        foreach (var word in keywords)
        {
            var DocsWithoutWord = _search.Search(word, invIndexDto);
            result.ExceptWith(DocsWithoutWord);
        }

        return result;
    }
}
