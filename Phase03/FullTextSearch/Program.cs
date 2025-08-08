using System.Diagnostics.CodeAnalysis;
using FullTextSearch.InvertedIndex.BuilderServices;
using FullTextSearch.InvertedIndex.SearchFeatures;
using FullTextSearch.Services.FileReaderService;
using FullTextSearch.Services.LoggerService;
using FullTextSearch.Services.QueryBuilder;
using FullTextSearch.Services.TokenizerService;

namespace FullTextSearch
{
    [ExcludeFromCodeCoverage]
    class Program
    {
        static void Main(string[] args)
        {
            var fileReader = new FileReader();
            var logger = new ConsoleLogger();
            var tokenizer = new Tokenizer();
            var documentAdder = new DocumentAdder(tokenizer);
            var invertedIndex = new InvertedIndexBuilder(documentAdder);

            var searchService = new SearchService(tokenizer);
            var advanceSearch = new AdvancedSearch();
            var singleWordExtractor = new SingleWordQueryExtractor();
            var app = new SearchApplication(
                fileReader,
                invertedIndex,
                logger,
                searchService,
                advanceSearch,
                singleWordExtractor);

            app.Run();
        }
    }
}