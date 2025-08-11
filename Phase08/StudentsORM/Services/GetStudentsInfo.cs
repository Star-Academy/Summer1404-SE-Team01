using StudentsORM.DbConfig;
using StudentsORM.DbConfig.Abstract;
using StudentsORM.DTO;
using StudentsORM.Services.Abstract;

namespace StudentsORM.Services;

public class GetStudentsInfo : IGetStudentsInfo
{
    private readonly IAppDbContextFactory _appDbContextFactory;
    private readonly IEnrollmentAverageGradeCalculator _enrollmentAverageGradeCalculator;

    public GetStudentsInfo(IAppDbContextFactory dbContextFactory, IEnrollmentAverageGradeCalculator enrollmentAverageGradeCalculator)
    {
        _appDbContextFactory = dbContextFactory;
        _enrollmentAverageGradeCalculator = enrollmentAverageGradeCalculator;
    }

    public IReadOnlyCollection<StudentWithAverageDto> GetTopStudents(int count = 3)
    {
        var averges = _enrollmentAverageGradeCalculator.calculateAverages(count);
        var topStudentIds = averges.Select(s => s.StudentId).ToList();
        var studentsContext = _appDbContextFactory.CreateStudentDbContext();
        var topStudentInfo = studentsContext.Students.Where(s => topStudentIds.Contains(s.Id)).ToList();

        var result = topStudentInfo
            .Join(averges,
                s => s.Id,
                dto => dto.StudentId,
                (s, dto) => new StudentWithAverageDto
                {
                    Id = s.Id,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    Average = dto.Average
                })
            .OrderByDescending(s => s.Average)
            .ToList();

        return result;
    }

}