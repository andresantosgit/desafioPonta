namespace BNB.ProjetoReferencia.Infrastructure.Http.Configuration;

public class HttpConfiguration
{
    public string Name { get; set; }
    public string BaseAddress { get; set; }
    public Dictionary<string, string> DefaultHeaders { get; set; }
}
