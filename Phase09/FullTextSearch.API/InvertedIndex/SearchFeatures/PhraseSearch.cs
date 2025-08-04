using FullTextSearch.API.InvertedIndex.Dtos;
using FullTextSearch.API.InvertedIndex.SearchFeatures.Abstractions;
using FullTextSearch.API.Services.TokenizerService;

public class PhraseSearch : ISearch
{
    private readonly ITokenizer _tokenizer;
    private readonly ISequentialValidator _sequentialValidator;

    public PhraseSearch(ITokenizer tokenizer, ISequentialValidator sequentialValidator)
    {
        _tokenizer = tokenizer ?? throw new ArgumentNullException(nameof(tokenizer));
        _sequentialValidator = sequentialValidator ?? throw new ArgumentNullException(nameof(sequentialValidator));
    }

    public SortedSet<string> Search(string phrase, InvertedIndexDto invIdxDto)
    {
        if (string.IsNullOrWhiteSpace(phrase))
        {
            return new SortedSet<string>();
        }

        var words = _tokenizer.Tokenize(phrase)?.ToList();
        if (words == null || words.Count == 0)
        {
            return new SortedSet<string>();
        }

        var docIdsContainingWords = new SortedSet<string>(invIdxDto.AllDocuments);
        foreach (var word in words)
        {
            if (invIdxDto.InvertedIndexMap.TryGetValue(word, out var docInfoSet))
            {
                docIdsContainingWords.IntersectWith(docInfoSet.Select(d => d.DocId));
            }
            else
            {
                return new SortedSet<string>();
            }
        }

        return _sequentialValidator.Validate(words, docIdsContainingWords, invIdxDto);
    }
}