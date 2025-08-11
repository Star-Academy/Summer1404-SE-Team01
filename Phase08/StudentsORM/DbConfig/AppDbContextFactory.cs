using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using StudentsORM.DbConfig.Abstract;

namespace StudentsORM.DbConfig;

[ExcludeFromCodeCoverage]
public class AppDbContextFactory: IAppDbContextFactory
{   
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    public AppDbContextFactory(IDbContextFactory<AppDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
    }
    
    public ICourseDbContext CreateCourseDbContext()
    {
        return _dbContextFactory.CreateDbContext();
    }

    public IStudentDbContext CreateStudentDbContext()
    {
        return _dbContextFactory.CreateDbContext();
    }

    public IEnrollmentDbContext CreateEnrollmentDbContext()
    {
        return _dbContextFactory.CreateDbContext();
    }
}