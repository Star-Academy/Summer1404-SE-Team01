using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace StudentsORM.Domain;

[ExcludeFromCodeCoverage]
public class Course
{
   [Key]
   public long Id { get; set; }
   public string Name { get; set; }
   
   public List<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}