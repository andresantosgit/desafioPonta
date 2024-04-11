using BNB.ProjetoReferencia.Core.Domain.Cliente.Entities;
using BNB.ProjetoReferencia.Core.Domain.Cliente.Interfaces;
using BNB.ProjetoReferencia.Core.Domain.WeatherForecast.Entities;
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
    /// <param name="id"></param>
    /// <param name="clienteRepository"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ClienteModel>> Get(
        [FromRoute] string id,
        [FromServices] IClienteRepository clienteRepository,
        CancellationToken cancellationToken)
    {
        var cliente = await clienteRepository.FindByIdInvestidorAsync(id, cancellationToken);
        if (cliente is null)
            return NoContent();
        return Ok(CriarModelo(cliente));
    }

    private ClienteModel CriarModelo(ClienteEntity entidade) => new(this, entidade);

}
