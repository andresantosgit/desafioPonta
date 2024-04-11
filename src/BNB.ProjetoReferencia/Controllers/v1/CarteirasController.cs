using BNB.ProjetoReferencia.Core.Common.Interfaces;
using BNB.ProjetoReferencia.Core.Domain.Carteira.Entities;
using BNB.ProjetoReferencia.Core.Domain.Carteira.Events;
using BNB.ProjetoReferencia.Core.Domain.Carteira.Interfaces;
using BNB.ProjetoReferencia.Extensions;
using BNB.ProjetoReferencia.Inputs;
using BNB.ProjetoReferencia.Models;
using Microsoft.AspNetCore.Mvc;

namespace BNB.ProjetoReferencia.Controllers.v1;

/// <summary>
/// Representa um cliente
/// </summary>
[ApiController]
[Route("api/v1/carteiras")]
public class CarteirasController : ControllerBase
{
    /// <summary>
    /// Obtem todas as carteiras de todos investidores
    /// </summary>
    /// <param name="carteiraRepository"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<CarteiraModel>>> Get(
        [FromServices] ICarteiraRepository carteiraRepository,
        CancellationToken cancellationToken)
    {
        var carteiras = await carteiraRepository.FindAllAsync(cancellationToken);
        if (!carteiras.Any())
            return NoContent();

        return Ok(carteiras.Select(x => CriarModelo(x)).ToList());
    }

    /// <summary>
    /// Obtem todas as carteiras de um investidor
    /// </summary>
    /// <param name="idInvestidor"></param>
    /// <param name="carteiraRepository"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("{idInvestidor}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<CarteiraModel>>> Get(
        [FromRoute] string idInvestidor,
        [FromServices] ICarteiraRepository carteiraRepository,
        CancellationToken cancellationToken)
    {
        idInvestidor = Uri.UnescapeDataString(idInvestidor);
        var carteiras = await carteiraRepository.FindAllByIdInvestidorAsync(idInvestidor, cancellationToken);
        if (!carteiras.Any())
            return NoContent();

        return Ok(carteiras.Select(x => CriarModelo(x)).ToList());
    }

    /// <summary>
    /// Exclui logicamente um determinado registro de carteira de um investidor
    /// </summary>
    /// <param name="idInvestidor"></param>
    /// <param name="id"></param>
    /// <param name="carteiraRepository"></param>
    /// <param name="excluirCarteiraEventHandler"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpDelete("{idInvestidor}/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CarteiraModel>> Delete(
        [FromRoute] string idInvestidor,
        [FromRoute] int id,
        [FromServices] ICarteiraRepository carteiraRepository,
        [FromServices] IRequestHandler<DomainEvent<CancelarCarteiraEvent>, CarteiraEntity> excluirCarteiraEventHandler,
        CancellationToken cancellationToken)
    {
        idInvestidor = Uri.UnescapeDataString(idInvestidor);
        var carteiras = await carteiraRepository.FindAllByIdInvestidorAsync(idInvestidor, cancellationToken);
        if (!carteiras.Any(x => x.Id == id))
            return NoContent();

        var evento = this.CriarEventoDominio<CancelarCarteiraEvent>(new(id, idInvestidor));
        await excluirCarteiraEventHandler.Handle(evento, cancellationToken);

        return Ok();
    }

    /// <summary>
    /// Cria uma manifestação de compra de ações
    /// </summary>
    /// <param name="input"></param>
    /// <param name="criarCarteiraEventHandler"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CarteiraModel>> Post(
        [FromBody] CriarCarteiraInput input,
        [FromServices] IRequestHandler<DomainEvent<CriarCarteiraEvent>, CarteiraEntity> criarCarteiraEventHandler,
        CancellationToken cancellationToken)
    {
        var evento = this.CriarEventoDominio<CriarCarteiraEvent>(input);
        var novaCarteira = await criarCarteiraEventHandler.Handle(evento, cancellationToken);
        return Ok(CriarModelo(novaCarteira));
    }

    private CarteiraModel CriarModelo(CarteiraEntity entidade) => new(this, entidade);
}
