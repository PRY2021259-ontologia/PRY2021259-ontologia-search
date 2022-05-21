using Microsoft.AspNetCore.Mvc;
using Ontologia.SPARQL.Server.Services;

namespace Ontologia.SPARQL.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly SearchService _searchService;

        public SearchController(IConfiguration configuration, SearchService searchService)
        {
            _configuration = configuration;
            _searchService = searchService;
        }

        [HttpGet]
        public ActionResult OntoSearch([FromQuery] string parameter)
        {
            var resources = _searchService.GetResources(parameter);
            return Ok(resources);
        }
    }
}
