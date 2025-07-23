using FullTextSearch.InvertedIndex.FilterSpecifications;
using FullTextSearch.InvertedIndex.QueryBuilder;
using FullTextSearch.InvertedIndex.SearchFeatures;
using FullTextSearch.Services.TokenizerService;

namespace FullTextSearch.InvertedIndex
{
    public class InvertedIndex : IInvertedIndexBuilder
    {
        private readonly ITokenizer _tokenizer;
        private SortedDictionary<string, SortedSet<string>> _invertedIndexMap;
        private ISearch _simpleSearch;
        private ISearch _advancedSearch;

        public SortedSet<string> AllDocuments { get; private set; }
        public SortedDictionary<string, SortedSet<string>> InvertedIndexMap => _invertedIndexMap;

        public InvertedIndex(ITokenizer tokenizer)
        {
            _tokenizer = tokenizer;
            _invertedIndexMap = new SortedDictionary<string, SortedSet<string>>();
            AllDocuments = new SortedSet<string>();
        }

        public void Build(Dictionary<string, string> documents)
        {
            _invertedIndexMap = new SortedDictionary<string, SortedSet<string>>();
            AllDocuments = new SortedSet<string>(documents.Keys);

            foreach (var (docId, contents) in documents)
            {
                var tokens = _tokenizer.Tokenize(contents);
                foreach (var word in tokens)
                {
                    if (!_invertedIndexMap.ContainsKey(word))
                    {
                        _invertedIndexMap[word] = new SortedSet<string>();
                    }
                    _invertedIndexMap[word].Add(docId);
                }
            }

            InitializeSearchComponents();
        }

        private void InitializeSearchComponents()
        {
            _simpleSearch = new InvertedIndexSimpleSearch(this);

            var queryExtractor = new QueryExtractor();

            _advancedSearch = new InvertedIndexAdvancedSearch(
                this,
                queryExtractor,
                new List<ISpecification>());
        }

        public IEnumerable<string> SearchWord(string word)
        {
            return _simpleSearch.Search(word);
        }

        public IEnumerable<string> AdvancedSearch(string query)
        {
            return _advancedSearch.Search(query);
        }
    }
}