using StudentsORM.Domain;

namespace StudentsORM.DTO;

public class DataDto
{
    public List<Enrollment> CourseGrades { get; set; }
    public List<Student> StudentsData { get; set; }
}
