using System.Text.RegularExpressions;
using FullTextSearch.Services.TokenizerService;

namespace FullTextSearch.InvertedIndex
{
    

    public class InvertedIndex : IInvertedIndex
    {
        private readonly SortedDictionary<string, SortedSet<string>> _invertedIndexMap = new();
        private readonly ITokenizer _tokenizer;
        private SortedSet<string> _docIds;

        public InvertedIndex(ITokenizer tokenizer)
        {
            _tokenizer = tokenizer;
        }

        public void BuildIndex(Dictionary<string, string> documents)
        {
            foreach ((var docId, var contents) in documents)
            {
                var tokens = _tokenizer.Tokenize(contents);
                    tokens.Aggregate(_invertedIndexMap, (current, word) =>
                {
                    if(!current.ContainsKey(word))
                    {
                        current.Add(word, new SortedSet<string>());
                    }
                    current[word].Add(docId);
                    return current;
                });
            }
            
            _docIds = new SortedSet<string>(documents.Keys);
        }

        public SortedSet<string> SearchWord(string word)
        {
            string upperWord = word.ToUpper();
            _invertedIndexMap.TryGetValue(upperWord, out SortedSet<string>? result);
            return result ?? new SortedSet<string>();
        }
    
        public SortedSet<string> AdvancedSearch(string query)
        {
         (var necessaryWords, var optionalWords, var excludedWords) = ExtractQueries(query);
            

            var result = new SortedSet<string>(_docIds);
            FindDocsIncludingNecessaryWords(result, necessaryWords);
            FindDocsIncludingAtLeastOneOptionalWords(result, optionalWords);
            FindDocsNotIncludingExcludedWords(result, excludedWords);
            
            return result;
        }

        private static (List<string> necessaryWords, List<string>OptionalWords, List<string> excludedWords)
            ExtractQueries(string query)
        {
            var words = Regex.Split(query!, @"\s+");
        
            var necessaryWords = words.Where(w => Regex.IsMatch(w, @"^[^-+]\w+")).ToList();
            var optionalWords = words.Where(w => Regex.IsMatch(w, @"^\+\w+")).ToList();
            var excludedWords = words.Where(w => Regex.IsMatch(w, @"^\-\w+")).ToList();
            
            return (necessaryWords, optionalWords, excludedWords);
        }

        private void FindDocsIncludingNecessaryWords(SortedSet<string> result, List<string> necessaryWords)
        {
            foreach (var word in necessaryWords)
            {
                var currentDocIds = SearchWord(word);
            
                result.IntersectWith(currentDocIds);
            }
        }

        private void FindDocsIncludingAtLeastOneOptionalWords(SortedSet<string> result, List<string> optionalWords)
        {
            var optionalDocIds = new SortedSet<string>();
            foreach (var word in optionalWords)
            {
                var currentDocIds = SearchWord(word);
                optionalDocIds.UnionWith(currentDocIds);
            }

            if (optionalDocIds.Any())
            {
                result.IntersectWith(optionalDocIds);
            }
        }
        
        private void FindDocsNotIncludingExcludedWords(SortedSet<string> result, List<string> excludedWords)
        {
            foreach (var word in excludedWords)
            {
                var currentDocIds = SearchWord(word);
                result.ExceptWith(currentDocIds);
            }
        }
        
    }
}