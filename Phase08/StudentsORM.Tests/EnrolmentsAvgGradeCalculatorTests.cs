using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using StudentsORM.DbConfig;
using StudentsORM.Domain;
using StudentsORM.DTO;
using StudentsORM.Services;

public class EnrolmentsAvgGradeCalculatorTests
{
    private static AppDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
       .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
       .Options;

        var context = new AppDbContext(options);
        context.Enrollments.AddRange(new List<Enrollment>
        {
            new Enrollment { StudentId = 1, CourseId = 1, Grade = 80 },
            new Enrollment { StudentId = 1, CourseId = 2, Grade = 90 },
            new Enrollment { StudentId = 2, CourseId = 5, Grade = 70 },
            new Enrollment { StudentId = 3, CourseId = 5, Grade = 100 }
        });

        context.SaveChanges();

        return context;
    }

    [Fact]
    public void CalculateAverages_ShouldReturnCorrectAverages()
    {
        // Arrange
        using var context = CreateInMemoryContext();

        var service = new EnrolmentsAvgGradeCalculator(context);

        // Act
        var result = service.calculateAverages();

        // Assert
        result.Should().HaveCount(3);
        result.Should().ContainEquivalentOf(new AveragesDto { StudentId = 1, Average = 85 });
        result.Should().ContainEquivalentOf(new AveragesDto { StudentId = 2, Average = 70 });
        result.Should().ContainEquivalentOf(new AveragesDto { StudentId = 3, Average = 100 });
    }

    [Fact]
    public void CalculateAverages_ShouldReturn_WithDescendingOrderByAverage()
    {
        // Arrange
        using var context = CreateInMemoryContext();

        var service = new EnrolmentsAvgGradeCalculator(context);

        // Act
        var result = service.calculateAverages();

        // Assert
        result.Should().HaveCount(3);
        result.Should().BeInDescendingOrder(r => r.Average);
    }

}