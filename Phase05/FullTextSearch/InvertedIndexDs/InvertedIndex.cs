using FullTextSearch.Services.TokenizerService;

namespace FullTextSearch.InvertedIndexDs
{
    public class InvertedIndex : IInvertedIndexBuilder
    {
        private readonly ITokenizer _tokenizer;
        public SortedDictionary<string, SortedSet<string>> InvertedIndexMap { get; private set; }
        public SortedSet<string> AllDocuments { get; private set; }

        public InvertedIndex(ITokenizer tokenizer)
        {
            _tokenizer = tokenizer;
            InvertedIndexMap = new SortedDictionary<string, SortedSet<string>>();
        }

        public void Build(Dictionary<string, string> documents)
        {
            AllDocuments = new SortedSet<string>(documents.Keys);

            foreach (var (docId, contents) in documents)
            {
                var tokens = _tokenizer.Tokenize(contents);
                foreach (var word in tokens)
                {
                    if (!InvertedIndexMap.ContainsKey(word))
                    {
                        InvertedIndexMap[word] = new SortedSet<string>();
                    }
                    InvertedIndexMap[word].Add(docId);
                }
            }

        }
    }
}