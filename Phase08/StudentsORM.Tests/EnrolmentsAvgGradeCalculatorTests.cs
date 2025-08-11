using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using StudentsORM.DbConfig;
using StudentsORM.DbConfig.Abstract;
using StudentsORM.Domain;
using StudentsORM.DTO;
using StudentsORM.Services;
using StudentsORM.Services.Abstract;

namespace StudentsORM.Tests;

public class EnrolmentsAvgGradeCalculatorTests
{
    
    private readonly IAppDbContextFactory _appDbContextFactory;
    private readonly IEnrollmentAverageGradeCalculator _sut;
    public EnrolmentsAvgGradeCalculatorTests()
    {
        _appDbContextFactory = Substitute.For<IAppDbContextFactory>();
        _sut = new EnrolmentsAvgGradeCalculator(_appDbContextFactory);

    }


    private IEnrollmentDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        // var context = _app
        var context = new AppDbContext(options);
        context.Enrollments.AddRange(new List<Enrollment>
        {
            new () { StudentId = 1, CourseId = 1, Grade = 80 },
            new () { StudentId = 1, CourseId = 2, Grade = 90 },
            new () { StudentId = 2, CourseId = 5, Grade = 70 },
            new () { StudentId = 3, CourseId = 5, Grade = 100 }
        });

        context.SaveChanges();

        return context;
    }

    [Fact]
    public void CalculateAverages_ShouldReturnCorrectAverages()
    {
        
        // Arrange
        _appDbContextFactory.CreateEnrollmentDbContext().Returns(CreateInMemoryContext());

        

        // Act
        var expected = _sut.calculateAverages();

        // Assert
        expected.Should().HaveCount(3);
        expected.Should().ContainEquivalentOf(new AveragesDto { StudentId = 1, Average = 85 });
        expected.Should().ContainEquivalentOf(new AveragesDto { StudentId = 2, Average = 70 });
        expected.Should().ContainEquivalentOf(new AveragesDto { StudentId = 3, Average = 100 });
    }

    [Fact]
    public void CalculateAverages_ShouldReturn_WithDescendingOrderByAverage()
    {
        // Arrange
        _appDbContextFactory.CreateEnrollmentDbContext().Returns(CreateInMemoryContext());

        // Act
        var expected = _sut.calculateAverages();

        // Assert
        expected.Should().HaveCount(3);
        expected.Should().BeInDescendingOrder(r => r.Average);
    }

}