using Microsoft.AspNetCore.Mvc;
using Ontologia.SPARQL.Server.Models;
using Ontologia.SPARQL.Server.Services;
using Swashbuckle.AspNetCore.Annotations;

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
        [SwaggerOperation(
            Summary = "Search into ontology",
            Description = "Search into the ontology with a parameter sent",
            OperationId = "SearchPlantDisease"
        )]
        [SwaggerResponse(200, "Parameter searched", typeof(IEnumerable<ResponseQuery>))]
        [ProducesResponseType(typeof(IEnumerable<ResponseQuery>), 200)]
        [Produces("application/json")]
        public ActionResult OntoSearch([FromQuery] string parameter)
        {
            var resources = _searchService.GetResources(parameter.ToLower());
            return Ok(resources);
        }

        [HttpGet("Guided/plaga")]
        [SwaggerOperation(
            Summary = "Obtain all symptoms from plaga infections",
            Description = "Send a list with symptoms data from plaga infections",
            OperationId = "GetSymptomsFromPlagas"
        )]
        [SwaggerResponse(200, "Symptoms from plagas", typeof(IEnumerable<SymptomQuery>))]
        [ProducesResponseType(typeof(IEnumerable<ResponseQuery>), 200)]
        [Produces("application/json")]
        public ActionResult GuidedSearchPlaga()
        {
            var resources = _searchService.GetSymptoms(InfeccionEnum.Plaga.ToString());
            return Ok(resources);
        }
        [HttpGet("Guided/enfermedad")]
        [SwaggerOperation(
            Summary = "Obtain all symptoms from enfermedad infection",
            Description = "Send a list with symptoms data from enfermedad infections",
            OperationId = "GetSymptomsFromEnfermedades"
        )]
        [SwaggerResponse(200, "Symptoms from enfermedades", typeof(IEnumerable<SymptomQuery>))]
        [ProducesResponseType(typeof(IEnumerable<SymptomQuery>), 200)]
        [Produces("application/json")]
        public ActionResult GuidedSearchEnfermedad()
        {
            var resources = _searchService.GetSymptoms(InfeccionEnum.Enfermedad.ToString());
            return Ok(resources);
        }

        [HttpGet("infeccion/{ontologyId}")]
        [SwaggerOperation(
            Summary = "Search into ontology by ontology id",
            Description = "Search into the ontology with the ontology id as parameter",
            OperationId = "SearchPlantDisease"
        )]
        [SwaggerResponse(200, "PlantDisease searched", typeof(IEnumerable<InfeccionGraph>))]
        [ProducesResponseType(typeof(IEnumerable<InfeccionGraph>), 200)]
        [Produces("application/json")]
        public ActionResult GetInfeccionData([FromRoute] string ontologyId)
        {
            var resource = _searchService.GetInfectionData(ontologyId);
            return Ok(resource);
        }
    }
}
