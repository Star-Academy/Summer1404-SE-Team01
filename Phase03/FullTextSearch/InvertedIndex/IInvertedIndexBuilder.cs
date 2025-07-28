using FullTextSearch.InvertedIndex.Dtos;

namespace FullTextSearch.InvertedIndex;

public interface IInvertedIndexBuilder
{
    InvertedIndexDto Build(Dictionary<string, string> documents);

}