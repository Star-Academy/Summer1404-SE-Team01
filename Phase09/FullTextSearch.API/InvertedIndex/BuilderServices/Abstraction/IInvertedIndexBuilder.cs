using FullTextSearch.API.InvertedIndex.Dtos;

namespace FullTextSearch.API.InvertedIndex.BuilderServices.Abstraction;

public interface IInvertedIndexBuilder
{
    InvertedIndexDto Build(Dictionary<string, string> documents);

}