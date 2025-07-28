using FullTextSearch.InvertedIndexDs.Dtos;

namespace FullTextSearch.InvertedIndexDs.FilterSpecifications.Abstractions;

public interface ISequentialPhrase
{
    void ValidateSequentiality(SortedSet<string> docIds, IEnumerable<string> phrase,  InvertedIndexDto dto);
}