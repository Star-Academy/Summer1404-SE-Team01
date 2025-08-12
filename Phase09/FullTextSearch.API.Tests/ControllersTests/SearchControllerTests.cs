using FluentAssertions;
using FullTextSearch.API.AppInitiator;
using FullTextSearch.API.Controllers;
using FullTextSearch.API.InvertedIndex.Dtos;
using FullTextSearch.API.InvertedIndex.SearchFeatures.Abstractions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace FullTextSearch.API.Tests.ControllersTests
{
    public class SearchControllerTests
    {
        private readonly IInvertedIndexInitiator _initiator = Substitute.For<IInvertedIndexInitiator>();
        private readonly IAdvanceSearch _advancedSearch = Substitute.For<IAdvanceSearch>();
        private readonly SearchController _sut;

        public SearchControllerTests()
        {
            _sut = new SearchController(_initiator, _advancedSearch);
        }

        [Fact]
        public void WordSearch_ShouldReturnBadRequest_WhenTermIsNullOrEmpty()
        {
            // Arrange
            string term = null;

            // Act
            var actionResult1 = _sut.WordSearch(null);
            var actionResult2 = _sut.WordSearch(string.Empty);
            var actionResult3 = _sut.WordSearch("   ");

            // Assert
            actionResult1.Result.Should().BeOfType<BadRequestObjectResult>();
            actionResult2.Result.Should().BeOfType<BadRequestObjectResult>();
            actionResult3.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void QuerySearch_ShouldReturnBadRequest_WhenQueryDtoIsNull()
        {
            // Arrange
            QueryDto queryDto = null;

            // Act
            var actionResult = _sut.QuerySearch(queryDto);

            // Assert
            actionResult.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void QuerySearch_ShouldReturnBadRequest_WhenRequiredTermsAreEmpty()
        {
            // Arrange
            var queryDto = new QueryDto();

            // Act
            var actionResult = _sut.QuerySearch(queryDto);

            // Assert
            actionResult.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void WordSearch_ShouldReturnOkWithExpected_WithGivenTerm()
        {
            // Arrange
            var term = "test";
            var invertedIndexDto = new InvertedIndexDto
            {
                InvertedIndexMap = new(),
                AllDocuments = new()
            };

            var expected = new HashSet<string> { "doc1", "doc2" };

            _initiator.GetData().Returns(invertedIndexDto);
            _advancedSearch.Search(
                Arg.Any<QueryDto>(),
                invertedIndexDto
            ).Returns(expected);

            // Act
            var actionResult = _sut.WordSearch(term);

            // Assert
            var okResult = actionResult.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void QuerySearch_ShouldReturnOkWithExpectedResult_WithGivenQueryDto()
        {
            // Arrange
            var query = new QueryDto { Required = { "term1" } };
            var invertedIndexDto = new InvertedIndexDto
            {
                InvertedIndexMap = new(),
                AllDocuments = new()
            };
            var expected = new HashSet<string> { "doc3", "doc4" };

            _initiator.GetData().Returns(invertedIndexDto);
            _advancedSearch.Search(
                query,
                invertedIndexDto
            ).Returns(expected);

            // Act
            var actionResult = _sut.QuerySearch(query);

            // Assert
            var okResult = actionResult.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(expected);
        }
    }
}
