using FullTextSearch.API.InvertedIndex.Dtos;

namespace FullTextSearch.API.InvertedIndex;

public interface IInvertedIndexBuilder
{
    InvertedIndexDto Build(Dictionary<string, string> documents);

}