using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace StudentsORM.Domain;
[ExcludeFromCodeCoverage]
public class Enrollment
{
    public int StudentId { get; set; }
    public long CourseId { get; set; }
    public double Grade { get; set; }
    
    [ForeignKey(nameof(StudentId))]
    public Student Student { get; set; }
    [ForeignKey(nameof(CourseId))]
    public Course Course { get; set; }
    
}
