using System.ComponentModel.DataAnnotations;

namespace StudentsORM.Domain;

public class Course
{
   [Key]
   public long Id { get; set; }
   public string Name { get; set; }
   
   public List<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}