using FullTextSearch.InvertedIndexDs;
using FullTextSearch.InvertedIndexDs.QueryBuilder;
using FullTextSearch.InvertedIndexDs.SearchFeatures;
using FullTextSearch.Services.FileReaderService;
using FullTextSearch.Services.LoggerService;
using FullTextSearch.Services.TokenizerService;

namespace FullTextSearch
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = new ConsoleLogger();
            var tokenizer = new Tokenizer();
            var fileReader = new FileReader();
            var invertedIndex = new InvertedIndex(tokenizer);

            var simpleSearch = new InvertedIndexSimpleSearch(invertedIndex);
            var queryExtractor = new QueryExtractor();
            var app = new SearchApplication(
                fileReader,
                invertedIndex,
                logger,
                simpleSearch,
                queryExtractor);

            app.Run();
        }
    }
}