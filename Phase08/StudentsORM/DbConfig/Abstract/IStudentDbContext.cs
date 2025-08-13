using Microsoft.EntityFrameworkCore;
using StudentsORM.Domain;

namespace StudentsORM.DbConfig.Abstract;

public interface IStudentDbContext: ICommonDbContext
{
    DbSet<Student> Students { get; set; }
}