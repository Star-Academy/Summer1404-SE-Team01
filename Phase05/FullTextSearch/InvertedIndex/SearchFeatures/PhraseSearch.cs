using FullTextSearch.InvertedIndex.Dtos;
using FullTextSearch.InvertedIndex.SearchFeatures.Abstractions;
using FullTextSearch.Services.TokenizerService;

namespace FullTextSearch.InvertedIndex.SearchFeatures;

public class PhraseSearch : ISearch
{
    private readonly ITokenizer _tokenizer;

    public PhraseSearch(ITokenizer tokenizer)
    {
        _tokenizer = tokenizer ?? throw new ArgumentNullException(nameof(tokenizer));
    }

    public SortedSet<string> Search(string phrase, InvertedIndexDto dto)
    {
        
        var words = _tokenizer.Tokenize(phrase).ToList();
        var docIdsContainsWords = new SortedSet<string>(dto.AllDocuments);
        
        foreach (var word in words)
        {
            if (dto.InvertedIndexMap.TryGetValue(word, out var docInfoSet))
            {
                var currentWordDocIds = docInfoSet.Select(d => d.DocId);

                docIdsContainsWords.IntersectWith(currentWordDocIds);
            }
            else
            {
                return new SortedSet<string>();
            }


        }
        
        
        var result = new SortedSet<string>();
        foreach (var docId in docIdsContainsWords)
        {
            var firstWord = words[0];
            var firstWordDocumentInfoCurrentDocId = dto.InvertedIndexMap[firstWord].Single(d => d.DocId == docId);
            var firstWordCurrentDocIdIndexes = firstWordDocumentInfoCurrentDocId.Indexes;
            var commonIndexesOfCurrentDocId = new SortedSet<long>(firstWordCurrentDocIdIndexes);
            
            bool isDocIdHasThisPhrase = true;
            for (int i = 1; i < words.Count; i++)
            {
                var word = words.ElementAt(i);
                var wordDocInfo = dto.InvertedIndexMap[word].FirstOrDefault(d => d.DocId == docId);
                var wordIndexesReducedI =  new SortedSet<long>(
                    wordDocInfo!.Indexes.Select(index => index - i)
                );
                commonIndexesOfCurrentDocId.IntersectWith(wordIndexesReducedI); 
                
                if (!commonIndexesOfCurrentDocId.Any())
                {
                    isDocIdHasThisPhrase = false;
                    break;
                }
            }
            
            if (isDocIdHasThisPhrase)
            {
                result.Add(docId);
            }
            
        }

        return result;

    }
}