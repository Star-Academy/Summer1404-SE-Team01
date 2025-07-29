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
        var results = new SortedSet<DocumentInfo>();
        for (int i = 0; i < words.Count(); i++)
        {
            if (dto.InvertedIndexMap.ContainsKey(words[i]))
            {
                var docInfo = new SortedSet<DocumentInfo>(dto.InvertedIndexMap[words[i]]);
                foreach (var info in docInfo)
                {
                    // Fix: Convert the result of Select to a SortedSet<long> instead of List<long>
                    info.Indexes = new SortedSet<long>(info.Indexes.Select(index => index - i));
                }
                if (i == 0)
                    results.UnionWith(docInfo);
                else
                    results.IntersectWith(docInfo);
            }
            else
                return new SortedSet<string>();
        }
        var resultSet = results.Select(d => d.DocId);
        return new SortedSet<string>(resultSet);
    }
}