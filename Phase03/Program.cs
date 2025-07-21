using FullTextSearch.Services.FileReaderService;
using FullTextSearch.Services.LoggerService;
using FullTextSearch.Services.TokenizerService;

namespace FullTextSearch;

class Program
{
        static void Main(string[] args)
        {
                var logger = new ConsoleLogger();
                var tokenizer = new Tokenizer();
                var fileReader = new FileReader(logger);
                var invertedIndex = new InvertedIndex.InvertedIndex(tokenizer);
                
                var app = new SearchApplication(fileReader, invertedIndex, logger);
                app.Run();
        }


}