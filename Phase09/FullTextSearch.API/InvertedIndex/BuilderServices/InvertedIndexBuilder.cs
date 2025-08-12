using FullTextSearch.API.InvertedIndex.BuilderServices.Abstraction;
using FullTextSearch.API.InvertedIndex.Dtos;

namespace FullTextSearch.API.InvertedIndex.BuilderServices
{
    public class InvertedIndexBuilder : IInvertedIndexBuilder
    {
        private IDocumentAdder _documentAdder;

        public InvertedIndexBuilder(IDocumentAdder documentAdder)
        {
            _documentAdder =  documentAdder ??  throw new ArgumentNullException(nameof(documentAdder));
        }

        public InvertedIndexDto Build(Dictionary<string, string> documents)
        {

            var invIdxDto = new InvertedIndexDto
            {
                InvertedIndexMap = new(),
                AllDocuments = new (documents.Keys)
            };
            foreach (var (docId, contents) in documents)
            {
                _documentAdder.AddDocument(docId, contents,  invIdxDto);
            }

            return invIdxDto;
        }
    }
}