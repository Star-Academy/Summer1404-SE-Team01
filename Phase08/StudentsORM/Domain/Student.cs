using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace StudentsORM.Domain;
[ExcludeFromCodeCoverage]
public class Student
{   
    [Key]
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    
    public string Fullname => $"{FirstName} {LastName}";
    
    public List<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
