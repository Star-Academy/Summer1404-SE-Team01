using Microsoft.EntityFrameworkCore;

namespace StudentsORM.DbConfig.Abstract;

public interface IAppDbContextFactory
{
  ICourseDbContext CreateCourseDbContext();
  
  IStudentDbContext CreateStudentDbContext();
  
  IEnrollmentDbContext CreateEnrollmentDbContext();
    
}