using Microsoft.EntityFrameworkCore;
using StudentsORM.Domain;

namespace StudentsORM.DbConfig.Abstract;

public interface IEnrollmentDbContext: ICommonDbContext
{
    DbSet<Enrollment> Enrollments { get; set; }
}