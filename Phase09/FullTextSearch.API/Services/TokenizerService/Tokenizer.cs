using System.Text.RegularExpressions;

namespace FullTextSearch.API.Services.TokenizerService;

public class Tokenizer : ITokenizer
{
    private readonly Regex _wordRegex = new(@"[\w]+", RegexOptions.Compiled);

    public IEnumerable<string> Tokenize(string content)
    {
        return _wordRegex.Matches(content.ToUpper())
            .Select(m => m.Value)
            .Where(w => !string.IsNullOrEmpty(w))
            .Select(w => w.ToUpper());
    }
}