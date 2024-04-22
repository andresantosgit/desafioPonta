using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace BNB.ProjetoReferencia.Infrastructure.Http.MessageHandler;

public class HttpMessageHandler : DelegatingHandler
{
    //private readonly IConfiguration Configuration;    
    //private readonly HttpClient client;
    //private string token;
    //public HttpMessageHandler(HttpClient httpClient, IConfiguration configuration)
    //{
    //    Configuration = configuration;            
    //    client = httpClient;
    //}

    //public async Task<bool> CheckTokens()
    //{
    //    var uri = GetUriById(id);
    //    using var response = await _httpClient.GetAsync("https://sso.dreads.bnb/auth/realms/Desenv/protocol/openid-connect/token");
    //    var response = await client.GetAsync();
    //    if (disco.IsError) throw new Exception(disco.Error);

    //    var result = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
    //    {
    //        Address = disco.TokenEndpoint,
    //        ClientId = Configuration["Settings:ClientID"],
    //        ClientSecret = Configuration["Settings:ClientSecret"]
    //    });

    //    if (result.IsError)
    //    {
    //        //Log("Error: " + result.Error);
    //        return false;
    //    }

    //    Tokens.AccessToken = result.AccessToken;
    //    Tokens.RefreshToken = result.RefreshToken;

    //    return true;
    //}

    //protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    //{
    //    request.SetBearerToken(Tokens.AccessToken);

    //    var response = await base.SendAsync(request, cancellationToken);

    //    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
    //    {
    //        if (await CheckTokens())
    //        {
    //            request.SetBearerToken(Tokens.AccessToken);

    //            response = await base.SendAsync(request, cancellationToken);
    //        }
    //    }

    //    return response;
    //}
}