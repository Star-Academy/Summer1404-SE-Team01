using StudentsORM.DTO;

namespace StudentsORM.Services.Abstract;

public interface IGetStudentsInfo
{
    IReadOnlyCollection<StudentWithAverageDto> GetTopStudents(int count = 3);
}