using FullTextSearch.InvertedIndexDs.Dtos;

namespace FullTextSearch.InvertedIndexDs.FilterSpecifications.Abstractions;

public interface ISpecification
{
    void FilterDocumentsByQuery(SortedSet<string> result, string query, InvertedIndexDto dto);
}
