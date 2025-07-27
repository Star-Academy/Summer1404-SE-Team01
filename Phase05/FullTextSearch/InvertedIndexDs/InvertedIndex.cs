using FullTextSearch.Services.TokenizerService;

namespace FullTextSearch.InvertedIndexDs
{
    public class InvertedIndex : IInvertedIndexBuilder
    {
        private readonly ITokenizer _tokenizer;
        public SortedDictionary<string, SortedSet<DocumentInfo>> InvertedIndexMap { get; private set; }
        public SortedSet<string> AllDocuments { get; private set; }

        public InvertedIndex(ITokenizer tokenizer)
        {
            _tokenizer = tokenizer;
            InvertedIndexMap = new SortedDictionary<string, SortedSet<DocumentInfo>>();
        }

        public void Build(Dictionary<string, string> documents)
        {
            AllDocuments = new SortedSet<string>(documents.Keys);
            foreach (var (docId, contents) in documents)
            {
                var tokens = _tokenizer.Tokenize(contents);
    
                foreach (var (word, index) in tokens.Select((w, i) => (w, i)))
                {
                    if (!InvertedIndexMap.ContainsKey(word))
                    {
                        InvertedIndexMap[word] = new SortedSet<DocumentInfo>();
                    }

                    DocumentInfo documentInfo;
                    if (InvertedIndexMap[word].All(docInfo => docInfo.DocId != docId))
                    {
                        documentInfo = new DocumentInfo { DocId = docId };
                        InvertedIndexMap[word].Add(documentInfo);
                    }
                    else
                    {
                        documentInfo = InvertedIndexMap[word].Single(docInfo => docInfo.DocId == docId);
                    }

                    documentInfo.Indexes.Add(index); 
                }
            }

        }
    }
}