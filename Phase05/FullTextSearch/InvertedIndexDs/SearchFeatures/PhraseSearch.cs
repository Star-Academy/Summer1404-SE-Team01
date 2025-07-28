using FullTextSearch.InvertedIndexDs.Dtos;
using FullTextSearch.InvertedIndexDs.FilterSpecifications.Abstractions;
using FullTextSearch.InvertedIndexDs.SearchFeatures.Abstractions;
using FullTextSearch.Services.TokenizerService;

namespace FullTextSearch.InvertedIndexDs.SearchFeatures;

public class PhraseSearch: ISearch
{
    private readonly ITokenizer _tokenizer;
    private readonly ISearch _search;
    private readonly ISequentialPhrase _sequentialPhrase;

    public PhraseSearch(ITokenizer tokenizer, ISearch simpleSearch, ISequentialPhrase sequentialPhrase)
    {
        _tokenizer = tokenizer ?? throw new ArgumentNullException(nameof(tokenizer));
        _search = simpleSearch ?? throw new ArgumentNullException(nameof(simpleSearch));
        _sequentialPhrase = sequentialPhrase;
    }
    
    public SortedSet<string> Search(string phrase, InvertedIndexDto dto)
    {
        var words = _tokenizer.Tokenize(phrase);
        var tempDocs = new SortedSet<string>(dto.AllDocuments);
        foreach (var word in words)
        {
            var currentDocIds =  _search.Search(word, dto);
            tempDocs.IntersectWith(currentDocIds);
        }
        
        _sequentialPhrase.ValidateSequentiality(tempDocs, words, dto);
        
        
        return tempDocs;
    }
}