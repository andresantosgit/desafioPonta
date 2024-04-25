using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace BNB.ProjetoReferencia.Infrastructure.Http.MessageHandler;

public class HttpMessageClientHandler : DelegatingHandler
{
    private readonly IConfiguration Configuration;
    private readonly TokenProvider Tokens;    
    private readonly HttpClient client;

    public HttpMessageClientHandler(HttpClient httpClient, IConfiguration configuration, TokenProvider tokens)
    {
        Configuration = configuration;
        Tokens = tokens;        
        client = httpClient;
    }

    public async Task<bool> CheckTokens()
    {
        //var disco = await client.GetDiscoveryDocumentAsync(Configuration["Settings:Authority"]);
        //if (disco.IsError) throw new Exception(disco.Error);

        var result = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
        {
            Address = "https://sso.dreads.bnb/auth/realms/Desenv/protocol/openid-connect/token",
            ClientId = "s493-backend-subscricao-servico",
            ClientSecret = "e6cde745-05c8-4490-8bdd-99589bea52b5"
        });

        if (result.IsError)
        {
            //Log("Error: " + result.Error);
            return false;
        }

        Tokens.AccessToken = result.AccessToken;
        Tokens.RefreshToken = result.RefreshToken;

        return true;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.SetBearerToken(Tokens.AccessToken);

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            if (await CheckTokens())
            {
                request.SetBearerToken(Tokens.AccessToken);

                response = await base.SendAsync(request, cancellationToken);
            }
        }

        return response;
    }    

}

public class TokenProvider
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}