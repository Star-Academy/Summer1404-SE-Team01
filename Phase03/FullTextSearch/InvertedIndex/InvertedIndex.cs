using FullTextSearch.Services.TokenizerService;

namespace FullTextSearch.InvertedIndex
{
    public class InvertedIndex : IInvertedIndexBuilder
    {
        private readonly ITokenizer _tokenizer;
        private readonly SortedDictionary<string, SortedSet<string>> _invertedIndexMap;

        public SortedSet<string> AllDocuments { get; private set; }
        public SortedDictionary<string, SortedSet<string>> InvertedIndexMap => _invertedIndexMap;

        public InvertedIndex(ITokenizer tokenizer)
        {
            _tokenizer = tokenizer;
            _invertedIndexMap = new();
            AllDocuments = new();
        }

        public void Build(Dictionary<string, string> documents)
        {
            AllDocuments = new(documents.Keys);

            foreach (var (docId, contents) in documents)
            {
                var tokens = _tokenizer.Tokenize(contents);
                foreach (var word in tokens)
                {
                    if (!_invertedIndexMap.TryGetValue(word, out SortedSet<string>? value))
                    {
                        value = new();
                        _invertedIndexMap[word] = value;
                    }

                    value.Add(docId);
                }
            }
        }
    }
}