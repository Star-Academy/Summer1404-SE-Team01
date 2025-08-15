namespace FullTextSearch.Services.TokenizerService
{
    public interface ITokenizer
    {
        IEnumerable<string> Tokenize(string content);
    }
}