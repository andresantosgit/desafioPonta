using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace BNB.ProjetoReferencia.Infrastructure.Http.MessageHandler;

public class HttpMessageClientHandler : DelegatingHandler
{
    private readonly IConfiguration Configuration;     
    private readonly HttpClient client;

    public HttpMessageClientHandler(HttpClient httpClient, IConfiguration configuration)
    {
        
    }
    

}
