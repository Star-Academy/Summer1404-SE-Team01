using FullTextSearch.API.AppInitiator;
using FullTextSearch.API.InvertedIndex.Dtos;
using FullTextSearch.API.InvertedIndex.FilterStrategies;
using FullTextSearch.API.InvertedIndex.FilterStrategies.Abstractions;
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
        private readonly ISearch _searchService;
        public SearchController(IInvertedIndexInitiator initiator, IAdvanceSearch advancedSearch, ISearch searchService)
        {
            _initiator = initiator;
            _advancedSearch = advancedSearch;
            _searchService = searchService;
        }


        [HttpGet]
        public ActionResult<HashSet<string>> Word(string term)
        {
            var invIdxDto = _initiator.GetData();
            QueryDto queryDto = new QueryDto();
            queryDto.Required.Add(term);
            var requiredStrategy = new RequiredStrategy(_searchService);
            var result = _advancedSearch.Search(queryDto, invIdxDto, new List<IFilterStrategy> { requiredStrategy });
            return Ok(result);
        }


        [HttpPost]
        public ActionResult<HashSet<string>> Post([FromBody] QueryDto query)
        {
            var invIdxDto = _initiator.GetData();
            var result = _advancedSearch.Search(query, invIdxDto, CreateFilterStrategies());
            return Ok(result);
        }

        private List<IFilterStrategy> CreateFilterStrategies()
        {
            return new List<IFilterStrategy>
            {
                new RequiredStrategy(_searchService),
                new OptionalStrategy(_searchService),
                new ExcludedStrategy(_searchService),
            };
        }
    }
}
