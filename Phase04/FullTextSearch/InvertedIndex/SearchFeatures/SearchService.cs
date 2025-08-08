using FullTextSearch.InvertedIndex.Dtos;
using FullTextSearch.InvertedIndex.SearchFeatures.Abstractions;
using FullTextSearch.Services.TokenizerService;

namespace FullTextSearch.InvertedIndex.SearchFeatures;

public class SearchService : ISearch
{
    private readonly ITokenizer _tokenizer;

    public SearchService(ITokenizer tokenizer)
    {
        _tokenizer = tokenizer ?? throw new ArgumentNullException(nameof(tokenizer));
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

        return docIdsContainingWords;
    }
}