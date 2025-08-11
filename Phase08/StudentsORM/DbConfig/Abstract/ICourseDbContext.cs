using Microsoft.EntityFrameworkCore;
using StudentsORM.Domain;

namespace StudentsORM.DbConfig.Abstract;

public interface ICourseDbContext
{
    DbSet<Course> Courses { get; set; }
}