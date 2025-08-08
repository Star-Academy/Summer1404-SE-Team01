using FullTextSearch.API.InvertedIndex.Dtos;
using FullTextSearch.API.InvertedIndex.SearchFeatures.Abstractions;
using FullTextSearch.API.Services.TokenizerService;

namespace FullTextSearch.API.InvertedIndex.SearchFeatures;

public class SearchService : ISearch
{
    private readonly ITokenizer _tokenizer;
    private readonly ISequentialFinder _sequentialFinder;

    public SearchService(ITokenizer tokenizer, ISequentialFinder sequentialValidator)
    {
        _tokenizer = tokenizer ?? throw new ArgumentNullException(nameof(tokenizer));
        _sequentialFinder = sequentialValidator ?? throw new ArgumentNullException(nameof(sequentialValidator));
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

        return _sequentialFinder.FindSequentialPhrase(words, docIdsContainingWords, invIdxDto);
    }
}