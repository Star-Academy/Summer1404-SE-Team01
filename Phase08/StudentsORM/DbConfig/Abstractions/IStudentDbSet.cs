using Microsoft.EntityFrameworkCore;
using StudentsORM.Domain;

namespace StudentsORM.DbConfig.Abstractions;

public interface IStudentDbSet
{
    public DbSet<Student> Students { get; set; }
}