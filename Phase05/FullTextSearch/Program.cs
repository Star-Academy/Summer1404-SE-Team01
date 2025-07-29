using FullTextSearch.InvertedIndex;
using FullTextSearch.InvertedIndex.QueryBuilder;
using FullTextSearch.InvertedIndex.SearchFeatures;
using FullTextSearch.Services.FileReaderService;
using FullTextSearch.Services.LoggerService;
using FullTextSearch.Services.TokenizerService;
using System.Diagnostics.CodeAnalysis;

namespace FullTextSearch
{
    [ExcludeFromCodeCoverage]
    class Program
    {
        static void Main(string[] args)
        {
            var logger = new ConsoleLogger();
            var tokenizer = new Tokenizer();
            var fileReader = new FileReader();
            var invertedIndex = new InvertedIndexBuilder(tokenizer);

            var simpleSearch = new SimpleSearch();
            var phraseSearch = new PhraseSearch(tokenizer);
            var queryExtractor = new QueryExtractor();
            var phraseQueryExtractor = new PhraseQueryExtractor();
            var app = new SearchApplication(
                fileReader,
                invertedIndex,
                logger,
                simpleSearch,
                queryExtractor,
                phraseSearch,
                phraseQueryExtractor);

            app.Run();
        }
    }
}