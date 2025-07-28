using FullTextSearch.InvertedIndexDs.Dtos;
using FullTextSearch.InvertedIndexDs.FilterSpecifications;
using FullTextSearch.InvertedIndexDs.FilterSpecifications.Abstractions;
using FullTextSearch.InvertedIndexDs.SearchFeatures.Abstractions;

namespace FullTextSearch.InvertedIndexDs.SearchFeatures;

public class InvertedIndexAdvancedSearch : ISearch
{
    private readonly List<ISpecification> _specifications;

    public InvertedIndexAdvancedSearch(List<ISpecification> specifications)
    {
        _specifications = specifications;
    }

    public SortedSet<string> Search(string query, InvertedIndexDto dto)
    {
        var result = new SortedSet<string>(dto.AllDocuments);

        foreach (var specification in _specifications)
        {
            specification.FilterDocumentsByQuery(result, query, dto);
        }
        return result;
    }
}