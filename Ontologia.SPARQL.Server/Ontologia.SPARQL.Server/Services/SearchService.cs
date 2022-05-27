using System.ComponentModel.DataAnnotations;
using System.Configuration.Internal;
using System.Drawing;
using Ontologia.SPARQL.Server.Models;
using VDS.RDF;
using VDS.RDF.Query;
using VDS.RDF.Storage;

namespace Ontologia.SPARQL.Server.Services
{
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

            FusekiConnector fuseki = new FusekiConnector(searchUrl);
            PersistentTripleStore pst = new PersistentTripleStore(fuseki);
            var sparqlQuery = new SparqlParameterizedString();
            sparqlQuery.Namespaces.AddNamespace("data", new Uri($"{dataUrl}#"));
            sparqlQuery.CommandText = "SELECT ?x ?Descripcion ?NombreComun ?NombreCientifico ?Tipo " +
                                      "WHERE {" +
                                      $"?x data:Descripcion ?Descripcion. FILTER regex(?Descripcion, \"{parameter}\", \"i\")" +
                                      "?x data:NombreComun ?NombreComun." +
                                      "?x data:Tipo ?Tipo." +
                                      "OPTIONAL { ?x data:NombreCientifico ?NombreCientifico. }"+
                                      "}";
            var result = pst.ExecuteQuery(sparqlQuery.ToString());

            var resultSet = (SparqlResultSet)result;
            if (resultSet.IsEmpty) return resources;
            foreach (var setResult in resultSet.Results)
            {
                var typeString = getTypeString(setResult["x"].ToString());
                var tempResult = new ResponseQuery()
                {
                    OntologyId = typeString,
                    Descripcion = setResult["Descripcion"].ToString() ?? "",
                    Nombre = setResult["NombreComun"].ToString() ?? "",
                    NombreCientifico = setResult["NombreCientifico"].ToString() ?? "",
                    TipoInfeccion = setResult["Tipo"].ToString() ?? ""
                };
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
                var typeString = getTypeString(setResult["x"].ToString());
                var flagDuplicated = resources.Exists(i => i.OntologyId.Contains(typeString));
                if (flagDuplicated) continue;
                var tempResult = new ResponseQuery()
                {
                    OntologyId = typeString,
                    Descripcion = setResult["Descripcion"].ToString() ?? "",
                    Nombre = setResult["NombreComun"].ToString() ?? "",
                    NombreCientifico = setResult["NombreCientifico"].ToString() ?? "",
                    TipoInfeccion = setResult["Tipo"].ToString() ?? ""
                };
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
                var typeString = getTypeString(setResult["x"].ToString());
                var flagDuplicated = resources.Exists(i => i.OntologyId.Contains(typeString));
                if (flagDuplicated) continue;
                var tempResult = new ResponseQuery()
                {
                    OntologyId = typeString,
                    Descripcion = setResult["Descripcion"].ToString() ?? "",
                    Nombre = setResult["NombreComun"].ToString() ?? "",
                    NombreCientifico = setResult["NombreCientifico"].ToString() ?? "",
                    TipoInfeccion = setResult["Tipo"].ToString() ?? ""
                };
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
                var typeString = getTypeString(setResult["x"].ToString());
                var flagDuplicated = resources.Exists(i => i.OntologyId.Contains(typeString));
                if (flagDuplicated) continue;
                var tempResult = new ResponseQuery()
                {
                    OntologyId = typeString,
                    Descripcion = setResult["Descripcion"].ToString() ?? "",
                    Nombre = setResult["NombreComun"].ToString() ?? "",
                    NombreCientifico = setResult["NombreCientifico"].ToString() ?? "",
                    TipoInfeccion = setResult["Tipo"].ToString() ?? ""
                };
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
            

            FusekiConnector fuseki = new FusekiConnector(searchUrl);
            PersistentTripleStore pst = new PersistentTripleStore(fuseki);
            var sparqlQuery = new SparqlParameterizedString();
            sparqlQuery.Namespaces.AddNamespace("data", new Uri($"{dataUrl}#"));
                        sparqlQuery.CommandText = "SELECT ?Descripcion ?NombreComun ?NombreCientifico ?Tipo " +
                                                  "WHERE { " +
                                                  $"data:{ontologyId} data:NombreComun ?NombreComun. " +
                                                  $"data:{ontologyId} data:Tipo ?Tipo. " +
                                                  $"data:{ontologyId} data:NombreCientifico ?NombreCientifico. " +
                                                  $"data:{ontologyId} data:Descripcion ?Descripcion. " +
                                                  "}";
            var result = pst.ExecuteQuery(sparqlQuery.ToString());

            var resultSet = (SparqlResultSet)result;
            if (resultSet.IsEmpty) return new InfeccionGraph();
            var setResult = resultSet.Results.FirstOrDefault();

            var resource = new InfeccionGraph()
            {
                Descripcion = setResult["Descripcion"].ToString(),
                Tipo = setResult["Tipo"].ToString(),
                NombreCientifico = setResult["NombreCientifico"].ToString(),
                NombreComun = setResult["NombreComun"].ToString(),
                InfeccionId = ontologyId
            };

            return resource;
        }
        public IEnumerable<SymptomQuery> GetSymptoms(string value)
        {
            var searchUrl = _configuration.GetSection("Ontologia:BaseUrl").Value;
            var dataUrl = _configuration.GetSection("Ontologia:DataUrl").Value;
            var resources = new List<SymptomQuery>();

            FusekiConnector fuseki = new FusekiConnector(searchUrl);
            PersistentTripleStore pst = new PersistentTripleStore(fuseki);
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
                var sintomaId = getTypeString(setResult["x"].ToString());
                var infeccionId = getTypeString(setResult["y"].ToString());
                var tempResult = new SymptomQuery()
                {
                    SintomaId = sintomaId,
                    SintomaComentario = setResult["Comentario"].ToString() ?? "",
                    InfeccionId = infeccionId,
                    InfeccionNombre = setResult["Infeccion"].ToString() ?? ""
                };
                resources.Add(tempResult);
            }
            return resources;
        }
    }
}
