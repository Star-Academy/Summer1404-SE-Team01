using FullTextSearch.API.AppInitiator;
using FullTextSearch.API.InvertedIndex.Dtos;
using FullTextSearch.API.InvertedIndex.SearchFeatures.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace FullTextSearch.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly IInvertedIndexInitiator _initiator;
        private readonly IAdvanceSearch _advancedSearch;
        public SearchController(IInvertedIndexInitiator initiator, IAdvanceSearch advancedSearch)
        {
            _initiator = initiator;
            _advancedSearch = advancedSearch;
        }


        [HttpGet]
        public ActionResult<HashSet<string>> WordSearch(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return BadRequest("Search term cannot be empty");
            }

            var invIdxDto = _initiator.GetData();
            var queryDto = new QueryDto();
            queryDto.Required.Add(term);
            var result = _advancedSearch.Search(queryDto, invIdxDto);
            return Ok(result);
        }


        [HttpPost]
        public ActionResult<HashSet<string>> QuerySearch([FromBody] QueryDto queryDto)
        {
            if (queryDto == null)
            {
                return BadRequest("Query cannot be null");
            }

            if (queryDto.Required.Count == 0 && queryDto.Optional.Count == 0 && queryDto.Excluded.Count == 0)
            {
                return BadRequest("At least one search term must be provided");
            }

            var invIdxDto = _initiator.GetData();
            var result = _advancedSearch.Search(queryDto, invIdxDto);
            return Ok(result);
        }
    }
}
