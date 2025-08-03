using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using StudentsORM.DbConfig;
using StudentsORM.Domain;
using StudentsORM.DTO;
using StudentsORM.Services;
using StudentsORM.Services.Abstract;
using Xunit;

namespace StudentsORM.Tests;

public class GetStudentsInfoTests
{
    private AppDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_" + Guid.NewGuid())
            .Options;

        var context = new AppDbContext(options);

        context.Students.AddRange(
            new Student { Id = 1, FirstName = "Ali", LastName = "Ahmadi" },
            new Student { Id = 2, FirstName = "Sara", LastName = "Mohammadi" },
            new Student { Id = 3, FirstName = "Akbar", LastName = "Ghaphari" },
            new Student { Id = 4, FirstName = "Mohandes", LastName = "Delester" }
        );

        context.SaveChanges();

        return context;
    }

    [Fact]
    public void GetStudents_ShouldReturnTopStudents_WithHighestAverage()
    {
        // Arrange
        var mockCalculator = Substitute.For<IEnrollmentAverageGradeCalculator>();
        var averageDtos = new List<AveragesDto>
        {
            new AveragesDto { StudentId = 1, Average = 20 },
            new AveragesDto { StudentId = 2, Average = 15 }
        };

        mockCalculator.calculateAverages(3).Returns(averageDtos);

        var dbContext = CreateInMemoryDbContext();
        var service = new GetStudentsInfo(dbContext, mockCalculator);

        // Act
        var result = service.GetTopStudents(3);

        // Assert
        result.Should().HaveCount(2);

        result.Should().ContainEquivalentOf(new StudentWithAverageDto
        {
            Id = 1,
            FirstName = "Ali",
            LastName = "Ahmadi",
            Average = 20
        });

        result.Should().ContainEquivalentOf(new StudentWithAverageDto
        {
            Id = 2,
            FirstName = "Sara",
            LastName = "Mohammadi",
            Average = 15
        });
    }
    
    [Fact]
    public void GetStudents_ShouldReturnTopStudents_WithDescendingOrderByAverage()
    {
        // Arrange
        var mockCalculator = Substitute.For<IEnrollmentAverageGradeCalculator>();
        var averageDtos = new List<AveragesDto>
        {
            new AveragesDto { StudentId = 1, Average = 20 },
            new AveragesDto { StudentId = 2, Average = 15 }
        };

        mockCalculator.calculateAverages(3).Returns(averageDtos);

        using var dbContext = CreateInMemoryDbContext();
        var service = new GetStudentsInfo(dbContext, mockCalculator);

        // Act
        var result = service.GetTopStudents(3).ToList();

        // Assert
        result.Should().HaveCount(2);

        result.Should().BeInDescendingOrder(r => r.Average);
    }
}