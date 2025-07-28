using FullTextSearch.InvertedIndexDs.Dtos;
using FullTextSearch.InvertedIndexDs.SearchFeatures.Abstractions;
using FullTextSearch.Services.TokenizerService;

namespace FullTextSearch.InvertedIndexDs.SearchFeatures;

public class PhraseSearch : ISearch
{
    private readonly ITokenizer _tokenizer;
    private readonly ISearch _simpleSearch;

    public PhraseSearch(ITokenizer tokenizer, ISearch simpleSearch)
    {
        _tokenizer = tokenizer;
        _simpleSearch = simpleSearch;
    }

    public SortedSet<string> Search(string input, InvertedIndexDto dto)
    {
        throw new NotImplementedException();
    }
}
