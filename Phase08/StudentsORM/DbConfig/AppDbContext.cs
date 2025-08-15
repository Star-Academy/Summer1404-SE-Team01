using Microsoft.EntityFrameworkCore;
using StudentsORM.Domain;
using System.Diagnostics.CodeAnalysis;
using StudentsORM.DbConfig.Abstract;

namespace StudentsORM.DbConfig;

[ExcludeFromCodeCoverage]
public class AppDbContext : DbContext, ICourseDbContext, IStudentDbContext, IEnrollmentDbContext
{

    public DbSet<Course> Courses { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }

    public AppDbContext() { }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured == false)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Database=students-project;Username=postgres;Password=poy post***");
        }

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Enrollment>()
            .HasKey(e => new { e.StudentId, e.CourseId });


        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.Student)
            .WithMany(s => s.Enrollments)
            .HasForeignKey(e => e.StudentId);

        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.Course)
            .WithMany(c => c.Enrollments)
            .HasForeignKey(e => e.CourseId);
    }

}