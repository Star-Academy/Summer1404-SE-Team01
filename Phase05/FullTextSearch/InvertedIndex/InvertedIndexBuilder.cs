using FullTextSearch.InvertedIndex.Dtos;
using FullTextSearch.Services.TokenizerService;

namespace FullTextSearch.InvertedIndex
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

            var invIdxDto = new InvertedIndexDto
            {
                InvertedIndexMap = new(),
                AllDocuments = new(),
            };
            foreach (var (docId, contents) in documents)
            {
                var tokens = _tokenizer.Tokenize(contents);

                foreach (var (word, index) in tokens.Select((w, i) => (w, i)))
                {
                    if (!invIdxDto.InvertedIndexMap.ContainsKey(word))
                    {
                        invIdxDto.InvertedIndexMap[word] = new SortedSet<DocumentInfo>();
                    }

                    DocumentInfo documentInfo;
                    if (invIdxDto.InvertedIndexMap[word].All(docInfo => docInfo.DocId != docId))
                    {
                        documentInfo = new DocumentInfo
                        {
                            DocId = docId,
                            Indexes = new()
                        };
                        invIdxDto.InvertedIndexMap[word].Add(documentInfo);
                    }
                    else
                    {
                        documentInfo = invIdxDto.InvertedIndexMap[word].Single(docInfo => docInfo.DocId == docId);
                    }

                    documentInfo.Indexes.Add(index);
                }


            }
            invIdxDto.AllDocuments = new SortedSet<string>(documents.Keys);
            return invIdxDto;
        }
    }
}