using StudentsORM.DTO;

namespace StudentsORM.Services.Abstract;

public interface IEnrollmentAverageGradeCalculator
{
    IReadOnlyCollection<AveragesDto> calculateAverages(int count = 3);
}