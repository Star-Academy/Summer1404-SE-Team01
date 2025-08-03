using Microsoft.EntityFrameworkCore;
using StudentsORM.Domain;

namespace StudentsORM.DbConfig.Abstractions;

public interface IEnrollmentDbSet
{
    public DbSet<Enrollment> Enrollments { get; }
}