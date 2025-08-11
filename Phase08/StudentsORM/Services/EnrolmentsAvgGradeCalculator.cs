using StudentsORM.DbConfig;
using StudentsORM.DbConfig.Abstract;
using StudentsORM.DTO;
using StudentsORM.Services.Abstract;

namespace StudentsORM.Services;

public class EnrolmentsAvgGradeCalculator : IEnrollmentAverageGradeCalculator
{
    private readonly IAppDbContextFactory  _contextFactory;

    public EnrolmentsAvgGradeCalculator(IAppDbContextFactory dbContextFactory)
    {
        _contextFactory = dbContextFactory;
    }

    public IReadOnlyCollection<AveragesDto> calculateAverages(int count = 4)
    {
        var context = _contextFactory.CreateEnrollmentDbContext();
        var averages = context.Enrollments
            .GroupBy(e => e.StudentId)
            .Select(g => new AveragesDto
            {
                StudentId = g.Key,
                Average = g.Average(e => e.Grade)
            }).OrderByDescending(dto => dto.Average).Take(count).ToList();

        return averages;
    }
}