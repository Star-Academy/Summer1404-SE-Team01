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
using System.Diagnostics.Metrics;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

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
            
            var tracingOtlpEndpoint = builder.Configuration["OTLP_ENDPOINT_URL"];
            var otel = builder.Services.AddOpenTelemetry();

            // Configure OpenTelemetry Resources with the application name
            otel.ConfigureResource(resource => resource
                .AddService(serviceName: builder.Environment.ApplicationName));

            // Add Metrics for ASP.NET Core and our custom metrics and export to Prometheus
            otel.WithMetrics(metrics => metrics
                // Metrics provider from OpenTelemetry
                .AddAspNetCoreInstrumentation()
                // TODO add metter
                // Metrics provides by ASP.NET Core in .NET 8
                .AddMeter("Microsoft.AspNetCore.Hosting")
                .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
                // Metrics provided by System.Net libraries
                .AddMeter("System.Net.Http")
                .AddMeter("System.Net.NameResolution")
                .AddPrometheusExporter());

            // Add Tracing for ASP.NET Core and our custom ActivitySource and export to Jaeger
            otel.WithTracing(tracing =>
            {
                tracing.AddAspNetCoreInstrumentation();
                tracing.AddHttpClientInstrumentation();
                // TODO tracing.AddSource(greeterActivitySource.Name);
                if (tracingOtlpEndpoint != null)
                {
                    tracing.AddOtlpExporter(otlpOptions => { otlpOptions.Endpoint = new Uri(tracingOtlpEndpoint); });
                }
                else
                {
                    tracing.AddConsoleExporter();
                }
            });

            var app = builder.Build();
            
            // Configure the Prometheus scraping endpoint
            app.MapPrometheusScrapingEndpoint();

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