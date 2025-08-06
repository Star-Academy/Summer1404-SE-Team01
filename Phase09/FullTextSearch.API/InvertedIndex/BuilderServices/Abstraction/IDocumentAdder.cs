using FullTextSearch.API.InvertedIndex.Dtos;

namespace FullTextSearch.API.InvertedIndex.BuilderServices.Abstraction;

public interface IDocumentAdder
{
    public void AddDocument(string docId, string contents, InvertedIndexDto invIdxDto);
}