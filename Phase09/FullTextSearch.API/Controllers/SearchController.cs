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
            var invIdxDto = _initiator.GetData();
            var queryDto = new QueryDto();
            queryDto.Required.Add(term);
            var result = _advancedSearch.Search(queryDto, invIdxDto);
            return Ok(result);
        }


        [HttpPost]
        public ActionResult<HashSet<string>> QuerySearch([FromBody] QueryDto queryDto)
        {
            var invIdxDto = _initiator.GetData();
            var result = _advancedSearch.Search(queryDto, invIdxDto);
            return Ok(result);
        }
    }
}
