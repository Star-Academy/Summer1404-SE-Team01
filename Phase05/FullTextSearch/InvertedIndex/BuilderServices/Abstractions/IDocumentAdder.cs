using FullTextSearch.InvertedIndex.Dtos;

namespace FullTextSearch.InvertedIndex.BuilderServices.Abstractions;

public interface IDocumentAdder
{
    public void AddDocument(string docId, string contents, InvertedIndexDto invIdxDto);
}