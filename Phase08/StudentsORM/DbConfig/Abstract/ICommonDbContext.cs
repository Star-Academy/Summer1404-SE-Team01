using Microsoft.EntityFrameworkCore.Diagnostics;

namespace StudentsORM.DbConfig.Abstract;

public interface ICommonDbContext : IDisposable
{
    public int SaveChanges();
}