using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using StudentsORM.DbConfig;
using StudentsORM.Domain;
using StudentsORM.DTO;
using StudentsORM.Services;

namespace StudentsORM;

[ExcludeFromCodeCoverage]
class Program
{
    static async Task Main(string[] args)
    {
        using var context = new AppDbContext();
        
        
        var avgCalcService = new EnrolmentsAvgGradeCalculator(context);
        var studentsInfoService = new GetStudentsInfo(context, avgCalcService);
        var result = studentsInfoService.GetTopStudents(3);
        
        
        
        Console.WriteLine("Top Students:");
        int rank = 1;
        foreach (var s in result)
        {
            Console.WriteLine($"{rank++}: {s.FirstName} {s.LastName} with average: {s.Average:F2}");
        }
    }
}