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

public class GetStudentsInfoTests
{
    private readonly IAppDbContextFactory _appDbContextFactory;
    private readonly IEnrollmentAverageGradeCalculator _avgGradeCalculator;
    private readonly IGetStudentsInfo _sut; 

    public GetStudentsInfoTests()
    {
        
        _appDbContextFactory = Substitute.For<IAppDbContextFactory>();
        _avgGradeCalculator = Substitute.For<IEnrollmentAverageGradeCalculator>();
        _sut = new GetStudentsInfo(_appDbContextFactory,  _avgGradeCalculator);
    }


    private AppDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
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
        var averageDtos = new List<AveragesDto>
        {
            new () { StudentId = 1, Average = 20 },
            new () { StudentId = 2, Average = 15 }
        };

        _avgGradeCalculator.calculateAverages(3).Returns(averageDtos);

        _appDbContextFactory.CreateStudentDbContext().Returns(CreateInMemoryDbContext());
        

        // Act
        var expected = _sut.GetTopStudents(3);

        // Assert
        expected.Should().HaveCount(2);

        expected.Should().ContainEquivalentOf(new StudentWithAverageDto
        {
            Id = 1,
            FirstName = "Ali",
            LastName = "Ahmadi",
            Average = 20
        });

        expected.Should().ContainEquivalentOf(new StudentWithAverageDto
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
        
        var averageDtos = new List<AveragesDto>
        {
            new () { StudentId = 1, Average = 20 },
            new () { StudentId = 2, Average = 15 }
        };

        _avgGradeCalculator.calculateAverages(3).Returns(averageDtos);

        _appDbContextFactory.CreateStudentDbContext().Returns(CreateInMemoryDbContext());
        

        // Act
        var expected = _sut.GetTopStudents(3).ToList();

        // Assert
        expected.Should().HaveCount(2);

        expected.Should().BeInDescendingOrder(r => r.Average);
    }
}