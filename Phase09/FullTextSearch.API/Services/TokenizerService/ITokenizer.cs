namespace FullTextSearch.API.Services.TokenizerService
{
    public interface ITokenizer
    {
        IEnumerable<string> Tokenize(string content);
    }
}