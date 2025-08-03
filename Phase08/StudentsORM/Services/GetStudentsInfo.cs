using StudentsORM.DbConfig;
using StudentsORM.DbConfig.Abstractions;
using StudentsORM.DTO;
using StudentsORM.Services.Abstract;

namespace StudentsORM.Services;

public class GetStudentsInfo : IGetStudentsInfo
{
    private readonly IStudentDbSet _context;
    private readonly IEnrollmentAverageGradeCalculator _enrollmentAverageGradeCalculator;

    public GetStudentsInfo(AppDbContext context, IEnrollmentAverageGradeCalculator enrollmentAverageGradeCalculator)
    {
        _context = context;
        _enrollmentAverageGradeCalculator = enrollmentAverageGradeCalculator;
    } 

    public IReadOnlyCollection<StudentWithAverageDto> GetTopStudents(int count = 10)
    {
        var averges = _enrollmentAverageGradeCalculator.calculateAverages(count);
        var topStudentIds = averges.Select(s => s.StudentId).ToList();
        var topStudentInfo = _context.Students.Where(s => topStudentIds.Contains(s.Id)).ToList();

        var result = topStudentInfo
            .Join(averges,
                s   => s.Id,
                dto => dto.StudentId,
                (s, dto) => new StudentWithAverageDto
                {
                    Id       = s.Id,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    Average  = dto.Average
                })
            .OrderByDescending(s => s.Average)
            .ToList();

        return result;
    }

}