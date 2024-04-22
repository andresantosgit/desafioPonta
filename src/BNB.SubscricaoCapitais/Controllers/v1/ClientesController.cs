using BNB.ProjetoReferencia.Core.Common.Interfaces;
using BNB.ProjetoReferencia.Core.Domain.Cliente.Entities;
using BNB.ProjetoReferencia.Core.Domain.Cliente.Events;
using BNB.ProjetoReferencia.Core.Domain.Cliente.Interfaces;
using BNB.ProjetoReferencia.Extensions;
using BNB.ProjetoReferencia.Inputs;
using BNB.ProjetoReferencia.Models;
using Microsoft.AspNetCore.Mvc;

namespace BNB.ProjetoReferencia.Controllers.v1;

/// <summary>
/// Representa um cliente
/// </summary>
[ApiController]
[Route("api/v1/clientes")]
public class ClientesController : ControllerBase
{
    /// <summary>
    /// Obtem as informaçãoes do cliente
    /// </summary>
    /// <param name="idInvestidor"></param>
    /// <param name="clienteRepository"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("{idInvestidor}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ClienteModel))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorModel))]
    public async Task<ActionResult<ClienteModel>> Get(
        [FromRoute] string idInvestidor,
        [FromServices] IClienteRepository clienteRepository,
        CancellationToken cancellationToken)
    {
        idInvestidor = Uri.UnescapeDataString(idInvestidor);
        var cliente = await clienteRepository.FindByIdInvestidorAsync(idInvestidor, cancellationToken);
        if (cliente is null)
            return NoContent();
        return Ok(CriarModelo(cliente));
    }

    /// <summary>
    /// Atualizar os dados do Investidor
    /// </summary>
    /// <param name="input"></param>
    /// <param name="atualizarClienteEventHandler"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ClienteModel))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorModel))]
    public async Task<ActionResult> Post(
        [FromBody] AtualizarClienteInput input,
        [FromServices] IRequestHandler<DomainEvent<AtualizarClienteEvent>, ClienteEntity> atualizarClienteEventHandler,
        CancellationToken cancellationToken)
    {
        var evento = this.CriarEventoDominio<AtualizarClienteEvent>(input);
        var cliente = await atualizarClienteEventHandler.Handle(evento, cancellationToken);
        return Ok(CriarModelo(cliente));
    }

    private ClienteModel CriarModelo(ClienteEntity entidade) => new(this, entidade);
}
