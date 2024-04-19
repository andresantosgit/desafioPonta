using BNB.ProjetoReferencia.Core.Domain.Cliente.Entities;
using BNB.ProjetoReferencia.Core.Domain.Cliente.Interfaces;
using BNB.ProjetoReferencia.Core.Domain.Cobranca.Entities;
using BNB.ProjetoReferencia.Core.Domain.Cobranca.Interfaces;
using BNB.ProjetoReferencia.Models;
using Microsoft.AspNetCore.Mvc;

namespace BNB.ProjetoReferencia.Controllers.v1;

/// <summary>
/// Representa uma cobranca
/// </summary>
[ApiController]
[Route("api/v1/cobranca")]
public class CobrancaController : ControllerBase
{
    /// <summary>
    /// Obtem as informaçãoes da cobranca
    /// </summary>
    /// <param name="txId"></param>
    /// <param name="cobrancaRepository"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("{txId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CobrancaModel>> Get(
        [FromRoute] string txId,
        [FromServices] ICobrancaRepository cobrancaRepository,
        CancellationToken cancellationToken)
    {        
        var cobranca = await cobrancaRepository.GetByTxId(txId, cancellationToken);
        if (cobranca is null)
            return NoContent();
        return Ok(CriarModelo(cobranca));
    }

    private CobrancaModel CriarModelo(CobrancaEntity entidade) => new(this, entidade);

}
