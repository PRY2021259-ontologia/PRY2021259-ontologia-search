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
            var searchUrl = _configuration.GetSection("Ontologia:Base_Url").Value;

            var resources = new List<ResponseQuery>();

            FusekiConnector fuseki = new FusekiConnector(searchUrl);
            PersistentTripleStore pst = new PersistentTripleStore(fuseki);
            var sparqlQuery = new SparqlParameterizedString();
            sparqlQuery.Namespaces.AddNamespace("data", new Uri($"{searchUrl}#"));
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
                    NombreRecurso = typeString,
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
                var flagDuplicated = resources.Exists(i => i.NombreRecurso.Contains(typeString));
                if (flagDuplicated) continue;
                var tempResult = new ResponseQuery()
                {
                    NombreRecurso = typeString,
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
                var flagDuplicated = resources.Exists(i => i.NombreRecurso.Contains(typeString));
                if (flagDuplicated) continue;
                var tempResult = new ResponseQuery()
                {
                    NombreRecurso = typeString,
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
                var flagDuplicated = resources.Exists(i => i.NombreRecurso.Contains(typeString));
                if (flagDuplicated) continue;
                var tempResult = new ResponseQuery()
                {
                    NombreRecurso = typeString,
                    Descripcion = setResult["Descripcion"].ToString() ?? "",
                    Nombre = setResult["NombreComun"].ToString() ?? "",
                    NombreCientifico = setResult["NombreCientifico"].ToString() ?? "",
                    TipoInfeccion = setResult["Tipo"].ToString() ?? ""
                };
                resources.Add(tempResult);
            }
            return resources;
        }
        public string getTypeString(string rawResource)
        {
            return rawResource.Substring(rawResource.IndexOf("#", StringComparison.Ordinal) + 1);
        }
    }
}
