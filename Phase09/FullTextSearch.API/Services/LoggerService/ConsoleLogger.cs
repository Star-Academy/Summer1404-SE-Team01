using System.Diagnostics.CodeAnalysis;

namespace FullTextSearch.API.Services.LoggerService;

[ExcludeFromCodeCoverage]
public class ConsoleLogger : ILogger
{
    public void LogInformation(string message) => Console.WriteLine($"[INFO] {message}");
    public void LogWarning(string message) => Console.WriteLine($"[WARN] {message}");
    public void LogError(string message) => Console.WriteLine($"[ERROR] {message}");
}