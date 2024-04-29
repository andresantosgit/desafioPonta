
using bnbauth=BNB.S095.BNBAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using BNB.ProjetoReferencia.Core.Domain.ExternalServices.Interfaces;
using BNB.ProjetoReferencia.Core.Domain.Util;
using BNB.ProjetoReferencia.Core.Common.Helper;
using IdentityModel.Client;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace BNB.ProjetoReferencia.Core.Domain.ExternalServices.Entities
{
    /// <summary>
    /// Classe de implementação do serviço de autorização
    /// </summary>
    public class TokenService : ITokenService
    {
        private readonly HttpClient _httpClient;
        public TokenService(IHttpClientFactory fatory, IConfiguration configuration)
        {
            _httpClient = fatory.CreateClient("cobranca");            
        }
        public async Task<string> GetToken()
        {
            string json;
            using (StreamReader r = new StreamReader("bnbauth.json"))
            {
                json = r.ReadToEnd();
            }

            Item items = JsonConvert.DeserializeObject<Item>(json);

            var result = await _httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = $"{items.EnderecoSeguranca}realms/{items.Ambiente}/protocol/openid-connect/token",
                ClientId = items.ClientId,
                ClientSecret = items.Credenciais.Secret
            });

            return result.AccessToken;
        }

    }

    public class Item
    {        
        [JsonProperty("auth-server-url")]
        public string EnderecoSeguranca { get; set; }
        [JsonProperty("realm")]
        public string Ambiente { get; set; }
        [JsonProperty("ssl-required")]
        public string Ssl { get; set; }
        [JsonProperty("resource")]
        public string ClientId { get; set; }
        [JsonProperty("credentials")]
        public Credenciais Credenciais { get; set; }        
    }

    public class Credenciais
    {
        [JsonProperty("secret")]
        public string Secret { get; set; }        

    }
}
