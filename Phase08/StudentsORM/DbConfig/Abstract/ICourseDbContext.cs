using Microsoft.EntityFrameworkCore;
using StudentsORM.Domain;

namespace StudentsORM.DbConfig.Abstract;

public interface ICourseDbContext: ICommonDbContext
{
    DbSet<Course> Courses { get; set; }
}