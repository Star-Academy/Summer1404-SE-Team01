using StudentsORM.DbConfig;
using StudentsORM.DTO;
using StudentsORM.Services.Abstract;

namespace StudentsORM.Services;

public class EnrolmentsAvgGradeCalculator : IEnrollmentAverageGradeCalculator
{
    private readonly AppDbContext _context;

    public EnrolmentsAvgGradeCalculator(AppDbContext context)
    {
        _context = context;
    }

    public IReadOnlyCollection<AveragesDto> calculateAverages(int count = 10)
    {
        var averages = _context.Enrollments
            .GroupBy(e => e.StudentId)
            .Select(g => new AveragesDto
            {
                StudentId = g.Key,
                Average = g.Average(e => e.Grade)
            }).OrderByDescending(dto => dto.Average).Take(count).ToList();

        return averages;
    }
}