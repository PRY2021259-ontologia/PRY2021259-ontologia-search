using Microsoft.AspNetCore.Mvc;
using Ontologia.SPARQL.Server.Models;
using Ontologia.SPARQL.Server.Services;

namespace Ontologia.SPARQL.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly SearchService _searchService;

        public SearchController(SearchService searchService)
        {
            _searchService = searchService;
        }

        [HttpGet]
        public ActionResult OntoSearch([FromQuery] string parameter)
        {
            var resources = _searchService.GetResources(parameter);
            return Ok(resources);
        }

        [HttpGet("Guided/plaga")]
        public ActionResult GuidedSearchPlaga()
        {
            var resources = _searchService.GetSymptoms(InfeccionEnum.Plaga.ToString());
            return Ok(resources);
        }
        [HttpGet("Guided/enfermedad")]
        public ActionResult GuidedSearchEnfermedad()
        {
            var resources = _searchService.GetSymptoms(InfeccionEnum.Enfermedad.ToString());
            return Ok(resources);
        }
    }
}
