using FullTextSearch.InvertedIndexDs.Dtos;

namespace FullTextSearch.InvertedIndexDs;

public interface IInvertedIndexBuilder
{
    InvertedIndexDto Build(Dictionary<string, string> documents);

}