using BNB.ProjetoReferencia.Core.Common.Attributes;
using BNB.ProjetoReferencia.Core.Common.Interfaces;
using BNB.ProjetoReferencia.Core.Domain.Cobranca.Entities;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using System.Text.Json;

namespace BNB.ProjetoReferencia.Infrastructure.Http.Repositories;

[Service(ServiceLifetime.Scoped, typeof(IRepository<CobrancaEntity, string>))]
public class CobrancaRepository
    : IRepository<CobrancaEntity, string>
{
    private readonly HttpClient _httpClient;

    #region Data Records

    internal record CobrancaModel(CalendarioModel Calendario, string Txid, int Revisao, LocModel Loc, string Location, string Status, DevedorModel Devedor, ValorModel Valor, string Chave, string SolicitacaoPagador, List<InfoAdicionalModel> InfoAdicionais);
    internal record CalendarioModel(DateTime Criacao, int Expiracao);
    internal record LocModel(int Id, string Location, string TipoCob);
    internal record DevedorModel(string Cnpj, string Nome);
    internal record ValorModel(string Original, int ModalidadeAlteracao);
    internal record InfoAdicionalModel(string Nome, string Valor);

    #endregion Data Records

    public CobrancaRepository(IHttpClientFactory fatory)
    {
        _httpClient = fatory.CreateClient("cobranca");
    }

    public async Task<CobrancaEntity> Add(CobrancaEntity entity, CancellationToken ctx)
    {
        var uri = GetUri();
        var content = JsonSerializer.Serialize(entity);

        using var response = await _httpClient.PostAsync(uri, new StringContent(content), ctx);
        if (!response.IsSuccessStatusCode) return default;

        var model = await response.Content.ReadFromJsonAsync<CobrancaModel>(cancellationToken: ctx);
        return Map(model);
    }

    public async Task<bool> AnyById(string id, CancellationToken ctx)
    {
        var uri = GetUriById(id);
        using var response = await _httpClient.GetAsync(uri, ctx);
        if (!response.IsSuccessStatusCode) return default;
        return await response.Content.ReadFromJsonAsync<CobrancaModel>(cancellationToken: ctx) is not null;
    }

    public async Task<CobrancaEntity> GetById(string id, CancellationToken ctx)
    {
        var uri = GetUriById(id);
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

        return new CobrancaEntity
        {
            Calendario = new Calendario
            {
                Criacao = model.Calendario.Criacao,
                Expiracao = model.Calendario.Expiracao
            },
            Revisao = model.Revisao,
            Loc = new Loc
            {
                Id = model.Loc.Id,
                Location = model.Loc.Location,
                TipoCob = model.Loc.TipoCob
            },
            Location = model.Location,
            Status = model.Status,
            Devedor = new Devedor
            {
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
            InfoAdicionais = model.InfoAdicionais.Select(info => new InfoAdicional
            {
                Nome = info.Nome,
                Valor = info.Valor
            }).ToList()
        };
    }

    internal CobrancaModel Map(CobrancaEntity entity)
    {
        if (entity == null) return null;

        return new CobrancaModel(
            new CalendarioModel(entity.Calendario.Criacao, entity.Calendario.Expiracao),
            entity.Id,
            entity.Revisao,
            new LocModel(entity.Loc.Id, entity.Loc.Location, entity.Loc.TipoCob),
            entity.Location,
            entity.Status,
            new DevedorModel(entity.Devedor.Cnpj, entity.Devedor.Nome),
            new ValorModel(entity.Valor.Original, entity.Valor.ModalidadeAlteracao),
            entity.Chave,
            entity.SolicitacaoPagador,
            entity.InfoAdicionais.Select(info => new InfoAdicionalModel(info.Nome, info.Valor)).ToList()
        );
    }


    private Uri GetUriById(string id)
        => new(_httpClient.BaseAddress ?? throw new NotSupportedException(), $"/api/cob/{id}");

    private Uri GetUri()
        => new(_httpClient.BaseAddress ?? throw new NotSupportedException(), $"/api/cob");
}
