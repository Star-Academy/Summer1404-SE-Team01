using FullTextSearch.InvertedIndex.Dtos;
using FullTextSearch.InvertedIndex.SearchFeatures.Abstractions;
using FullTextSearch.Services.TokenizerService;

namespace FullTextSearch.InvertedIndex.SearchFeatures;

public class SearchService : ISearch
{
    private readonly ITokenizer _tokenizer;
    private readonly ISequentialPhraseFinder _sequentialPhraseFinder;

    public SearchService(ITokenizer tokenizer, ISequentialPhraseFinder sequentialValidator)
    {
        _tokenizer = tokenizer ?? throw new ArgumentNullException(nameof(tokenizer));
        _sequentialPhraseFinder = sequentialValidator ?? throw new ArgumentNullException(nameof(sequentialValidator));
    }

    public HashSet<string> Search(string input, InvertedIndexDto invIdxDto)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return new HashSet<string>();
        }

        var words = _tokenizer.Tokenize(input).ToList();

        var docIdsContainingWords = new HashSet<string>(invIdxDto.AllDocuments);
        foreach (var word in words)
        {
            if (invIdxDto.InvertedIndexMap.TryGetValue(word, out var docInfoSet))
            {
                docIdsContainingWords.IntersectWith(docInfoSet.Select(d => d.DocId));
            }
            else
            {
                return new HashSet<string>();
            }
        }

        if (words.Count == 1)
        {
            return docIdsContainingWords;
        }

        return _sequentialPhraseFinder.FindSequentialPhrase(words, docIdsContainingWords, invIdxDto);
    }
}