
using FullTextSearch.InvertedIndexDs.Dtos;
using FullTextSearch.InvertedIndexDs.FilterSpecifications.Abstractions;

namespace FullTextSearch.InvertedIndexDs.FilterSpecifications;

public class OptionalPhraseSpecification : ISpecification
{
    

    public void FilterDocumentsByQuery(SortedSet<string> result, string query, InvertedIndexDto dto)
    {
        throw new NotImplementedException();
    }
}
