
namespace StudentsORM.DTO;

public class StudentWithAverageDto
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public double Average { get; set; }
    
    public string Fullname => $"{FirstName} {LastName}";
}