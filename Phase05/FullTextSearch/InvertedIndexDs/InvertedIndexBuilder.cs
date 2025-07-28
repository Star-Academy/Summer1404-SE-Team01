using FullTextSearch.InvertedIndexDs.Dtos;
using FullTextSearch.Services.TokenizerService;

namespace FullTextSearch.InvertedIndexDs
{
    public class InvertedIndexBuilder : IInvertedIndexBuilder
    {
        private readonly ITokenizer _tokenizer;

        public InvertedIndexBuilder(ITokenizer tokenizer)
        {
            _tokenizer = tokenizer;
        }

        public InvertedIndexDto Build(Dictionary<string, string> documents)
        {

            var dto = new InvertedIndexDto
            {
                InvertedIndexMap = new(),
                AllDocuments = new(),
            };
            foreach (var (docId, contents) in documents)
            {
                var tokens = _tokenizer.Tokenize(contents);
    
                foreach (var (word, index) in tokens.Select((w, i) => (w, i)))
                {
                    if (!dto.InvertedIndexMap.ContainsKey(word))
                    {
                        dto.InvertedIndexMap[word] = new SortedSet<DocumentInfo>();
                    }

                    DocumentInfo documentInfo;
                    if (dto.InvertedIndexMap[word].All(docInfo => docInfo.DocId != docId))
                    {
                        documentInfo = new DocumentInfo { DocId = docId };
                        dto.InvertedIndexMap[word].Add(documentInfo);
                    }
                    else
                    {
                        documentInfo = dto.InvertedIndexMap[word].Single(docInfo => docInfo.DocId == docId);
                    }

                    documentInfo.Indexes.Add(index); 
                }

                
            }
            dto.AllDocuments = new SortedSet<string>(documents.Keys);
            return dto;
        }
    }
}