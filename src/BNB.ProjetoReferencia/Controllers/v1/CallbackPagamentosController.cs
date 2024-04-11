using BNB.ProjetoReferencia.Core.Common.Interfaces;
using BNB.ProjetoReferencia.Core.Domain.Carteira.Entities;
using BNB.ProjetoReferencia.Core.Domain.Carteira.Events;
using BNB.ProjetoReferencia.Extensions;
using BNB.ProjetoReferencia.Inputs;
using BNB.ProjetoReferencia.Models;
using Microsoft.AspNetCore.Mvc;

namespace BNB.ProjetoReferencia.Controllers.v1;

/// <summary>
/// Representa um pagamento
/// </summary>
[ApiController]
[Route("api/v1/callback-pagamentos")]
public class CallbackPagamentosController : ControllerBase
{
    /// <summary>
    /// Valida o novo status do pagamento
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post(
        CancellationToken cancellationToken)
    {
        return Ok();
    }
}