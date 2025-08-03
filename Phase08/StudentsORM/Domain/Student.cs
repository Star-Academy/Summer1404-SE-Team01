using System.ComponentModel.DataAnnotations;

namespace StudentsORM.Domain;

public class Student
{   
    [Key]
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    
    public string Fullname => $"{FirstName} {LastName}";
    
    public List<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
