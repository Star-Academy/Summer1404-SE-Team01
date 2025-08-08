using FullTextSearch.InvertedIndex.Dtos;
using FullTextSearch.InvertedIndex.FilterStrategies.Abstractions;
using FullTextSearch.InvertedIndex.SearchFeatures.Abstractions;

namespace FullTextSearch.InvertedIndex.FilterStrategies;

public class ExcludedStrategy : IFilterStrategy
{

    private readonly ISearch _search;

    public ExcludedStrategy(ISearch searchService)
    {
        _search = searchService ?? throw new ArgumentNullException(nameof(searchService));
    }
    public HashSet<string> FilterDocumentsByQuery(QueryDto queryDto, InvertedIndexDto invIndexDto)
    {
        var result = new HashSet<string>(invIndexDto.AllDocuments);

        foreach (var word in queryDto.Excluded)
        {
            var docsWithoutWord = _search.Search(word, invIndexDto);
            result.ExceptWith(docsWithoutWord);
        }

        return result;
    }
}