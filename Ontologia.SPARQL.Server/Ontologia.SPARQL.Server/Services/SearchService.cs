using System.ComponentModel.DataAnnotations;
using System.Configuration.Internal;
using System.Drawing;
using Ontologia.SPARQL.Server.Models;
using VDS.RDF;
using VDS.RDF.Query;
using VDS.RDF.Storage;

namespace Ontologia.SPARQL.Server.Services;

public class SearchService
{
    private readonly IConfiguration _configuration;

    public SearchService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IEnumerable<ResponseQuery> GetResources(string parameter)
    {
        var searchUrl = _configuration.GetSection("Ontologia:BaseUrl").Value;
        var dataUrl = _configuration.GetSection("Ontologia:DataUrl").Value;
        var resources = new List<ResponseQuery>();

        var fuseki = new FusekiConnector(searchUrl);
        var pst = new PersistentTripleStore(fuseki);
        var sparqlQuery = new SparqlParameterizedString();
        sparqlQuery.Namespaces.AddNamespace("data", new Uri($"{dataUrl}#"));
        sparqlQuery.CommandText = "SELECT ?x ?Descripcion ?NombreComun ?NombreCientifico ?Tipo " +
                                  "WHERE {" +
                                  $"?x data:Descripcion ?Descripcion. FILTER regex(?Descripcion, \"{parameter}\", \"i\")" +
                                  "?x data:NombreComun ?NombreComun." +
                                  "?x data:Tipo ?Tipo." +
                                  "OPTIONAL { ?x data:NombreCientifico ?NombreCientifico. }" +
                                  "}";
        var result = pst.ExecuteQuery(sparqlQuery.ToString());

        var resultSet = (SparqlResultSet)result;
        if (resultSet.IsEmpty) return resources;
        foreach (var setResult in resultSet.Results)
        {
            var ontologyId = getTypeString(setResult["x"].ToString());
            var tempResult = convertSparqlResultToResponseQuery(setResult, ontologyId);
            resources.Add(tempResult);
        }

        sparqlQuery.CommandText = "SELECT ?x ?Descripcion ?NombreComun ?NombreCientifico ?Tipo " +
                                  "WHERE { " +
                                  $"?x data:NombreComun ?NombreComun. FILTER regex(?NombreComun, \"{parameter}\", \"i\") " +
                                  "?x data:Descripcion ?Descripcion. " +
                                  "?x data:Tipo ?Tipo. " +
                                  "OPTIONAL { ?x data:NombreCientifico ?NombreCientifico. } " +
                                  "}";

        result = pst.ExecuteQuery(sparqlQuery.ToString());
        resultSet = (SparqlResultSet)result;
        if (resultSet.IsEmpty) return resources;
        foreach (var setResult in resultSet.Results)
        {
            var ontologyId = getTypeString(setResult["x"].ToString());
            var flagDuplicated = resources.Exists(i => i.OntologyId.Contains(ontologyId));
            if (flagDuplicated) continue;
            var tempResult = convertSparqlResultToResponseQuery(setResult, ontologyId);
            resources.Add(tempResult);
        }

        sparqlQuery.CommandText = "SELECT ?x ?Descripcion ?NombreComun ?NombreCientifico ?Tipo " +
                                  "WHERE { " +
                                  $"?x data:Tipo ?Tipo. FILTER regex(?Tipo, \"{parameter}\", \"i\") " +
                                  "?x data:Descripcion ?Descripcion. " +
                                  "?x data:NombreComun ?NombreComun. " +
                                  "OPTIONAL { ?x data:NombreCientifico ?NombreCientifico. } " +
                                  "}";
        result = pst.ExecuteQuery(sparqlQuery.ToString());
        resultSet = (SparqlResultSet)result;
        if (resultSet.IsEmpty) return resources;
        foreach (var setResult in resultSet.Results)
        {
            var ontologyId = getTypeString(setResult["x"].ToString());
            var flagDuplicated = resources.Exists(i => i.OntologyId.Contains(ontologyId));
            if (flagDuplicated) continue;
            var tempResult = convertSparqlResultToResponseQuery(setResult, ontologyId);
            resources.Add(tempResult);
        }

        sparqlQuery.CommandText = "SELECT ?x ?Descripcion ?NombreComun ?NombreCientifico ?Tipo " +
                                  "WHERE { " +
                                  $"?x data:NombreCientifico ?NombreCientifico. FILTER regex(?NombreCientifico, \"{parameter}\", \"i\") " +
                                  "?x data:Descripcion ?Descripcion. " +
                                  "?x data:NombreComun ?NombreComun. " +
                                  "?x data:Tipo ?Tipo. " +
                                  "}";

        result = pst.ExecuteQuery(sparqlQuery.ToString());
        resultSet = (SparqlResultSet)result;
        if (resultSet.IsEmpty) return resources;
        foreach (var setResult in resultSet.Results)
        {
            var ontologyId = getTypeString(setResult["x"].ToString());
            var flagDuplicated = resources.Exists(i => i.OntologyId.Contains(ontologyId));
            if (flagDuplicated) continue;
            var tempResult = convertSparqlResultToResponseQuery(setResult, ontologyId);
            resources.Add(tempResult);
        }

        return resources;
    }

    private string getTypeString(string rawResource)
    {
        return rawResource.Substring(rawResource.IndexOf("#", StringComparison.Ordinal) + 1);
    }

    public InfeccionGraph GetInfectionData(string ontologyId)
    {
        var searchUrl = _configuration.GetSection("Ontologia:BaseUrl").Value;
        var dataUrl = _configuration.GetSection("Ontologia:DataUrl").Value;


        var fuseki = new FusekiConnector(searchUrl);
        var pst = new PersistentTripleStore(fuseki);
        var sparqlQuery = new SparqlParameterizedString();
        sparqlQuery.Namespaces.AddNamespace("data", new Uri($"{dataUrl}#"));
        sparqlQuery.CommandText = "SELECT ?Descripcion ?NombreComun ?NombreCientifico ?Tipo ?AfectaUnPorcentajeDeHasta " +
                                  "WHERE { " +
                                  $"data:{ontologyId} data:AfectaUnPorcentajeDeHasta ?AfectaUnPorcentajeDeHasta. " +
                                  $"data:{ontologyId} data:NombreComun ?NombreComun. " +
                                  $"data:{ontologyId} data:Tipo ?Tipo. " +
                                  $"data:{ontologyId} data:NombreCientifico ?NombreCientifico. " +
                                  $"data:{ontologyId} data:Descripcion ?Descripcion. " +
                                  "}";
        var result = pst.ExecuteQuery(sparqlQuery.ToString());

        var resultSet = (SparqlResultSet)result;
        if (resultSet.IsEmpty) return new InfeccionGraph();
        var setResult = resultSet.Results.FirstOrDefault();
        if (setResult == null) return new InfeccionGraph();
        var resource = convertSparqlResultToInfeccionGraph(setResult, ontologyId);
        resource.Sintomas = GetSintomaDetail(resource.InfeccionId);
        resource.AgentesCausales = GetAgenteCausalDetail(resource.InfeccionId);
        return resource;
    }

    private IEnumerable<SintomaData> GetSintomaDetail(string infeccionId)
    {
        var sintomaDataList = new List<SintomaData>();
        var searchUrl = _configuration.GetSection("Ontologia:BaseUrl").Value;
        var dataUrl = _configuration.GetSection("Ontologia:DataUrl").Value;


        var fuseki = new FusekiConnector(searchUrl);
        var pst = new PersistentTripleStore(fuseki);
        var sparqlQuery = new SparqlParameterizedString();
        sparqlQuery.Namespaces.AddNamespace("data", new Uri($"{dataUrl}#"));

        sparqlQuery.CommandText = "SELECT DISTINCT ?y " +
                                  "WHERE {" +
                                  $"data:{infeccionId} data:tiene_sintoma ?y." +
                                  "}";
        var result = pst.ExecuteQuery(sparqlQuery.ToString());
        var resultSintomaSet = (SparqlResultSet)result;
        if (!resultSintomaSet.IsEmpty)
            foreach (var sparqlResult in resultSintomaSet.Results)
            {
                var sintomaId = getTypeString(sparqlResult["y"].ToString());
                sparqlQuery.CommandText = "SELECT DISTINCT ?Nombre ?Descripcion " +
                                          "WHERE {" +
                                          $"data:{sintomaId} data:Nombre ?Nombre. " +
                                          $"data:{sintomaId} data:Descripcion ?Descripcion." +
                                          "}";
                var SintomaResult = pst.ExecuteQuery(sparqlQuery.ToString());
                var resultSet = (SparqlResultSet)SintomaResult;
                var setResult = resultSet.Results.FirstOrDefault();
                if (setResult == null) continue;
                var sintomaTemp = new SintomaData()
                {
                    Descripcion = setResult["Descripcion"].ToString() ?? "",
                    Nombre = setResult["Nombre"].ToString() ?? ""
                };
                sintomaDataList.Add(sintomaTemp);
            }

        return sintomaDataList;
    }

    private IEnumerable<AgenteCausalData> GetAgenteCausalDetail(string infeccionId)
    {
        var agenteCausalDataList = new List<AgenteCausalData>();
        var searchUrl = _configuration.GetSection("Ontologia:BaseUrl").Value;
        var dataUrl = _configuration.GetSection("Ontologia:DataUrl").Value;


        var fuseki = new FusekiConnector(searchUrl);
        var pst = new PersistentTripleStore(fuseki);
        var sparqlQuery = new SparqlParameterizedString();
        sparqlQuery.Namespaces.AddNamespace("data", new Uri($"{dataUrl}#"));

        sparqlQuery.CommandText = "SELECT DISTINCT ?y " +
                                  "WHERE {" +
                                  $"data:{infeccionId} data:tiene_agente_causal ?y." +
                                  "}";
        var result = pst.ExecuteQuery(sparqlQuery.ToString());
        var resultAgenteCausalSet = (SparqlResultSet)result;
        if (resultAgenteCausalSet.IsEmpty) return agenteCausalDataList;
        foreach (var sparqlResult in resultAgenteCausalSet.Results)
        {
            var agenteCausalId = getTypeString(sparqlResult["y"].ToString());
            sparqlQuery.CommandText = "SELECT DISTINCT ?Nombre ?Descripcion ?Tipo ?NombreCientifico " +
                                      "WHERE {" +
                                      $"data:{agenteCausalId} data:Nombre ?Nombre. " +
                                      $"data:{agenteCausalId} data:Descripcion ?Descripcion." +
                                      $"data:{agenteCausalId} data:Tipo ?Tipo. " +
                                      $"data:{agenteCausalId} data:NombreCientifico ?NombreCientifico. " +
                                      "}";
            var agenteResult = pst.ExecuteQuery(sparqlQuery.ToString());
            var resultSet = (SparqlResultSet)agenteResult;
            var setResult = resultSet.Results.FirstOrDefault();
            if (setResult == null) continue;
            var agenteTemp = new AgenteCausalData()
            {
                Descripcion = setResult["Descripcion"].ToString() ?? "",
                Nombre = setResult["Nombre"].ToString() ?? "",
                NombreCientifico = setResult["NombreCientifico"].ToString() ?? "",
                Tipo = setResult["Tipo"].ToString() ?? ""
            };
            agenteCausalDataList.Add(agenteTemp);
        }

        return agenteCausalDataList;
    }

    public IEnumerable<SintomaQuery> GetSymptoms(string value)
    {
        var searchUrl = _configuration.GetSection("Ontologia:BaseUrl").Value;
        var dataUrl = _configuration.GetSection("Ontologia:DataUrl").Value;
        var resources = new List<SintomaQuery>();

        var fuseki = new FusekiConnector(searchUrl);
        var pst = new PersistentTripleStore(fuseki);
        var sparqlQuery = new SparqlParameterizedString();
        sparqlQuery.Namespaces.AddNamespace("data", new Uri($"{dataUrl}#"));
        sparqlQuery.Namespaces.AddNamespace("rdfs", new Uri("http://www.w3.org/2000/01/rdf-schema#"));
        sparqlQuery.CommandText = "SELECT ?x ?Comentario ?y ?Infeccion " +
                                  "WHERE { " +
                                  $"?y data:Tipo \"{value}\". " +
                                  "?x data:es_sintoma_de ?y. " +
                                  "?y data:NombreComun ?Infeccion. " +
                                  "?x rdfs:comment ?Comentario. }";
        var result = pst.ExecuteQuery(sparqlQuery.ToString());

        var resultSet = (SparqlResultSet)result;
        if (resultSet.IsEmpty) return resources;
        foreach (var setResult in resultSet.Results)
        {
            if (setResult == null) continue;
            var tempResult = ConvertSparqlResultToSymptomQuery(setResult);
            resources.Add(tempResult);
        }

        return resources;
    }

    private ResponseQuery convertSparqlResultToResponseQuery(SparqlResult result, string ontologyId)
    {
        var tempResult = new ResponseQuery()
        {
            OntologyId = ontologyId,
            Descripcion = result["Descripcion"].ToString() ?? "",
            Nombre = result["NombreComun"].ToString() ?? "",
            NombreCientifico = result["NombreCientifico"].ToString() ?? "",
            TipoInfeccion = result["Tipo"].ToString() ?? ""
        };
        return tempResult;
    }

    private InfeccionGraph convertSparqlResultToInfeccionGraph(SparqlResult result, string ontologyId)
    {
        var decimalFromText = result["AfectaUnPorcentajeDeHasta"].ToString().IndexOf("^");
        var decimalString = "0";
        if (decimalFromText != 0)
        {
            decimalString = result["AfectaUnPorcentajeDeHasta"].ToString().Substring(0, decimalFromText);
        }
        
        var afectaATemp = decimal.Parse(decimalString);
        var resource = new InfeccionGraph()
        {
            Descripcion = result["Descripcion"].ToString(),
            AfectaA = afectaATemp == 0? null: afectaATemp,
            Tipo = result["Tipo"].ToString(),
            NombreCientifico = result["NombreCientifico"].ToString(),
            NombreComun = result["NombreComun"].ToString(),
            InfeccionId = ontologyId
        };
        return resource;
    }

    private SintomaQuery ConvertSparqlResultToSymptomQuery(SparqlResult result)
    {
        var sintomaId = getTypeString(result["x"].ToString() ?? "");
        var infeccionId = getTypeString(result["y"].ToString() ?? "");
        var tempResult = new SintomaQuery()
        {
            SintomaId = sintomaId,
            SintomaComentario = result["Comentario"].ToString() ?? "",
            InfeccionId = infeccionId,
            InfeccionNombre = result["Infeccion"].ToString() ?? ""
        };
        return tempResult;
    }
}