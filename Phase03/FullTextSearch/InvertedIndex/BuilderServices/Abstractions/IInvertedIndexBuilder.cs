using FullTextSearch.InvertedIndex.Dtos;

namespace FullTextSearch.InvertedIndex.BuilderServices.Abstractions;

public interface IInvertedIndexBuilder
{
    InvertedIndexDto Build(Dictionary<string, string> documents);

}