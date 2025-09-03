
using System.Diagnostics.CodeAnalysis;
using FullTextSearch.API.AppInitiator;
using FullTextSearch.API.InvertedIndex.BuilderServices;
using FullTextSearch.API.InvertedIndex.BuilderServices.Abstraction;
using FullTextSearch.API.InvertedIndex.FilterStrategies;
using FullTextSearch.API.InvertedIndex.FilterStrategies.Abstractions;
using FullTextSearch.API.InvertedIndex.SearchFeatures;
using FullTextSearch.API.InvertedIndex.SearchFeatures.Abstractions;
using FullTextSearch.API.Services.FileReaderService;
using FullTextSearch.API.Services.TokenizerService;

namespace FullTextSearch.API
{
    [ExcludeFromCodeCoverage]
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var services = builder.Services;
            services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddSingleton<IFileReader, FileReader>();
            services.AddSingleton<ITokenizer, Tokenizer>();
            services.AddSingleton<IInvertedIndexBuilder, InvertedIndexBuilder>();
            services.AddSingleton<IDocumentAdder, DocumentAdder>();
            services.AddSingleton<ISearch, SearchService>();
            services.AddSingleton<ISequentialPhraseFinder, SequentialPhraseFinder>();
            services.AddSingleton<IFilterStrategy, ExcludedStrategy>();
            services.AddSingleton<IFilterStrategy, OptionalStrategy>();
            services.AddSingleton<IFilterStrategy, RequiredStrategy>();
            services.AddSingleton<IAdvanceSearch, AdvancedSearch>();
            services.AddSingleton<IInvertedIndexInitiator, InvertedIndexInitiator>();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
