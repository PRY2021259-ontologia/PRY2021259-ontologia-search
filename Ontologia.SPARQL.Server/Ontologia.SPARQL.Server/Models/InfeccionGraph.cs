namespace Ontologia.SPARQL.Server.Models;

public class InfeccionGraph
{
    public string InfeccionId { get; set; }
    public string NombreComun { get; set; }
    public string Tipo { get; set; }
    public string NombreCientifico { get; set; }
    public string Descripcion { get; set; }
    public IEnumerable<SintomaData> Sintomas { get; set; }
    public IEnumerable<AgenteCausalData> AgentesCausales { get; set; }
}