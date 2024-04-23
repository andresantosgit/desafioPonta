using Azure;
using BNB.ProjetoReferencia.Core.Common.Attributes;
using BNB.ProjetoReferencia.Core.Common.Interfaces;
using BNB.ProjetoReferencia.Core.Domain.Cobranca.Entities;
using BNB.ProjetoReferencia.Core.Domain.Cobranca.Interfaces;
using BNB.ProjetoReferencia.Infrastructure.Http.Configuration;
using BNB.S095.BNBAuth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using System.Text.Json;
using BNB.S095.BNBAuth;
using IdentityModel.Client;

namespace BNB.ProjetoReferencia.Infrastructure.Http.Repositories;

[Service(ServiceLifetime.Scoped, typeof(ICobrancaRepository))]
public class CobrancaRepository
    : ICobrancaRepository
{
    private readonly HttpClient _httpClient;
    private readonly string _chave;

    #region Data Records

    internal record CobrancaModel(CalendarioModel Calendario, string Txid, int Revisao, LocModel Loc, string Location, string Status, DevedorModel Devedor, ValorModel Valor, string Chave, string SolicitacaoPagador, List<InfoAdicionalModel> InfoAdicionais, string PixCopiaECola);
    internal record CalendarioModel(DateTime Criacao, int Expiracao);
    internal record LocModel(int Id, string Location, string TipoCob);
    internal record DevedorModel(string Cpf, string Cnpj, string Nome);
    internal record ValorModel(string Original, int ModalidadeAlteracao);
    internal record InfoAdicionalModel(string Nome, string Valor);

    #endregion Data Records

    public CobrancaRepository(IHttpClientFactory fatory, IConfiguration configuration)
    {
        _httpClient = fatory.CreateClient("cobranca");
        _chave = configuration["CobrancaChave"];        
    }

    public async Task<CobrancaEntity> Add(CobrancaEntity entity, CancellationToken ctx)
    {        
        var uri = GetUri();
        entity.Chave = _chave;

        var serializeOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        var content = JsonSerializer.Serialize(entity, serializeOptions);

        var result = await _httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
        {
            Address = "https://sso.dreads.bnb/auth/realms/Desenv/protocol/openid-connect/token",
            ClientId = "s493-backend-subscricao-servico",
            ClientSecret = "e6cde745-05c8-4490-8bdd-99589bea52b5"
        });

        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", result.AccessToken);

        using var response = await _httpClient.PostAsync(uri, new StringContent(content), ctx);
        if (!response.IsSuccessStatusCode) return default;

        var model = await response.Content.ReadFromJsonAsync<CobrancaModel>(cancellationToken: ctx);
        return Map(model);
    }

    public async Task<bool> AnyById(string id, CancellationToken ctx)
    {
        var uri = GetUriById(id);

        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ContextoSeguranca.GetCredencialAplicacao().Token);

        using var response = await _httpClient.GetAsync(uri, ctx);
        if (!response.IsSuccessStatusCode) return default;
        return await response.Content.ReadFromJsonAsync<CobrancaModel>(cancellationToken: ctx) is not null;
    }

    public async Task<CobrancaEntity> GetByTxId(string txId, CancellationToken ctx)
    {
        var uri = GetUriById(txId);

        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ContextoSeguranca.GetCredencialAplicacao().Token);

        using var response = await _httpClient.GetAsync(uri, ctx);
        if (!response.IsSuccessStatusCode) return default;
        var model = await response.Content.ReadFromJsonAsync<CobrancaModel>(cancellationToken: ctx);
        return Map(model);
    }

    public Task<CobrancaEntity> Update(CobrancaEntity entity, CancellationToken ctx)
    {
        throw new NotImplementedException();
    }

    internal CobrancaEntity Map(CobrancaModel model)
    {
        if (model == null) return null;

        var cobrancaRetorno =  new CobrancaEntity
        {
            Calendario = new Calendario
            {
                Criacao = model.Calendario.Criacao,
                Expiracao = model.Calendario.Expiracao
            },
            Revisao = model.Revisao,
            
            Location = model.Location,
            Status = model.Status,
            Devedor = new Devedor
            {
                Cpf = model.Devedor.Cpf,
                Cnpj = model.Devedor.Cnpj,
                Nome = model.Devedor.Nome
            },
            Valor = new Valor
            {
                Original = model.Valor.Original,
                ModalidadeAlteracao = model.Valor.ModalidadeAlteracao
            },
            Chave = model.Chave,
            SolicitacaoPagador = model.SolicitacaoPagador,
            TxId = model.Txid,
            PixCopiaECola = model.PixCopiaECola
            
        };

        if (model.Loc != null)
        {
            cobrancaRetorno.Loc = new Loc
            {
                Id = model.Loc.Id,
                Location = model.Loc.Location,
                TipoCob = model.Loc.TipoCob
            };
        }

        if (model.InfoAdicionais != null)
        {            
            cobrancaRetorno.InfoAdicionais = new List<InfoAdicional>(
                model.InfoAdicionais.Select(info => new InfoAdicional
                {
                    Nome = info.Nome,
                    Valor = info.Valor
                }).ToList()
            );
        }
        return cobrancaRetorno;
    }

    internal CobrancaModel Map(CobrancaEntity entity)
    {
        if (entity == null) return null;

        return new CobrancaModel(
            new CalendarioModel(entity.Calendario.Criacao, entity.Calendario.Expiracao),
            entity.TxId,
            entity.Revisao,
            new LocModel(entity.Loc.Id, entity.Loc.Location, entity.Loc.TipoCob),
            entity.Location,
            entity.Status,
            new DevedorModel(entity.Devedor.Cpf, entity.Devedor.Cnpj, entity.Devedor.Nome),
            new ValorModel(entity.Valor.Original, entity.Valor.ModalidadeAlteracao),
            entity.Chave,
            entity.SolicitacaoPagador,
            entity.InfoAdicionais.Select(info => new InfoAdicionalModel(info.Nome, info.Valor)).ToList(),
            entity.PixCopiaECola = entity.PixCopiaECola
        );
    }


    private Uri GetUriById(string id)
        => new(_httpClient.BaseAddress ?? throw new NotSupportedException(), $"/cob/{id}");

    private Uri GetUri()
        => new(_httpClient.BaseAddress ?? throw new NotSupportedException(), $"/cob");   

}
