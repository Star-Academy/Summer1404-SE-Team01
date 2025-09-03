using Microsoft.EntityFrameworkCore;
using StudentsORM.Domain;

namespace StudentsORM.DbConfig.Abstract;

public interface IStudentDbContext:  IDisposable
{
    DbSet<Student> Students { get; set; }
}