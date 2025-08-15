using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StudentsORM.DbConfig;
using StudentsORM.DbConfig.Abstract;
using StudentsORM.Services;
using StudentsORM.Services.Abstract;

namespace StudentsORM;

[ExcludeFromCodeCoverage]
class Program
{
    static async Task Main(string[] args)
    {
        
        var services = new ServiceCollection();

        
        services.AddDbContextFactory<AppDbContext>(options =>
        {
            options.UseNpgsql("Host=localhost;Database=students-project;Username=postgres;Password=poy post***");
        });

        
        services.AddScoped<IAppDbContextFactory, AppDbContextFactory>();

        
        services.AddScoped<IEnrollmentAverageGradeCalculator, EnrolmentsAvgGradeCalculator>();
        services.AddScoped<IGetStudentsInfo, GetStudentsInfo>();

        
        var serviceProvider = services.BuildServiceProvider();

        
        var studentsInfoService = serviceProvider.GetRequiredService<IGetStudentsInfo>();
        var result = studentsInfoService.GetTopStudents();

        Console.WriteLine("Top Students:");
        int rank = 1;
        foreach (var s in result)
        {
            Console.WriteLine($"{rank++}: {s.FirstName} {s.LastName} with average: {s.Average:F2}");
        }
    }
}