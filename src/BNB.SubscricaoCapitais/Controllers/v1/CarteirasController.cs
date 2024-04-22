using BNB.ProjetoReferencia.Core.Common.Interfaces;
using BNB.ProjetoReferencia.Core.Domain.Carteira.Entities;
using BNB.ProjetoReferencia.Core.Domain.Carteira.Events;
using BNB.ProjetoReferencia.Core.Domain.Carteira.Interfaces;
using BNB.ProjetoReferencia.Core.Domain.Cliente.Entities;
using BNB.ProjetoReferencia.Core.Domain.Cliente.Interfaces;
using BNB.ProjetoReferencia.Extensions;
using BNB.ProjetoReferencia.Inputs;
using BNB.ProjetoReferencia.Models;
using Microsoft.AspNetCore.Mvc;
using QRCoder;

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
    /// <param name="clienteRepository"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CarteiraModel>))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorModel))]
    public async Task<ActionResult> Get(
        [FromServices] ICarteiraRepository carteiraRepository,
        [FromServices] IClienteRepository clienteRepository,
        CancellationToken cancellationToken)
    {
        var carteiras = await carteiraRepository.FindAllAsync(cancellationToken);
        if (!carteiras.Any())
            return NoContent();

        var clientesIdsInvestidor = carteiras
            .Select(x => x.IdInvestidor)
            .Distinct()
            .Select(x => clienteRepository.FindByIdInvestidorAsync(x, cancellationToken).Result)
            .ToArray();
        
        return Ok(carteiras.Select(x => CriarModelo(x, clientesIdsInvestidor.FirstOrDefault(y => y.IdInvestidor == x.IdInvestidor))).ToList());
    }

    /// <summary>
    /// Obtem todas as carteiras de um investidor
    /// </summary>
    /// <param name="idInvestidor"></param>
    /// <param name="carteiraRepository"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("{idInvestidor}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CarteiraModel>))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorModel))]
    public async Task<ActionResult> Get(
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
    /// Retorna QrCode para pagamento
    /// </summary>
    /// <param name="idCarteira"></param>
    /// <returns></returns>
    [HttpGet("qrCode/{idCarteira}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorModel))]
    public async Task<ActionResult> Get(
        [FromRoute] int idCarteira,
        [FromServices] ICarteiraRepository carteiraRepository,
        CancellationToken cancellationToken)
    {
        var carteira = carteiraRepository.GetById(idCarteira);
        if (carteira == null)
            return NoContent();

        QRCodeGenerator qrGenerator = new QRCodeGenerator();
        QRCodeData qrCodeData = qrGenerator.CreateQrCode(carteira.PixCopiaECola, QRCodeGenerator.ECCLevel.Q);
        BitmapByteQRCode qrCode = new BitmapByteQRCode(qrCodeData);
        string qrCodeBase64 = Convert.ToBase64String(qrCode.GetGraphic(10));

        return Ok(qrCodeBase64);
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
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CarteiraModel))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorModel))]
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
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CarteiraModel))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorModel))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorModel))]
    public async Task<ActionResult<CarteiraModel>> Post(
        [FromBody] CriarCarteiraInput input,
        [FromServices] IRequestHandler<DomainEvent<CriarCarteiraEvent>, CarteiraEntity> criarCarteiraEventHandler,
        CancellationToken cancellationToken)
    {
        var evento = this.CriarEventoDominio<CriarCarteiraEvent>(input);
        var novaCarteira = await criarCarteiraEventHandler.Handle(evento, cancellationToken);
        return Ok(CriarModelo(novaCarteira));
    }

    /// <summary>
    /// Obter o manifestação da compra de ações
    /// </summary>
    /// <param name="idCarteira"></param>
    /// <param name="carteiraRepository"></param>
    /// <param name="clienteRepository"></param>
    /// <param name="pdfGenerator"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("manifesto/{idCarteira}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorModel))]
    public async Task<ActionResult> ObterManifesto(
        [FromRoute] int idCarteira,
        [FromServices] ICarteiraRepository carteiraRepository,
        [FromServices] IClienteRepository clienteRepository,
        [FromServices] IPDFGenerator pdfGenerator,
        CancellationToken cancellationToken)
    {
        var carteira = carteiraRepository.GetById(idCarteira);
        if (carteira is null)
            return NoContent();

        var cliente = await clienteRepository.FindByIdInvestidorAsync(carteira.IdInvestidor, cancellationToken);
        if (cliente is null)
            return NoContent();

        var html = (await System.IO.File.ReadAllTextAsync("./manifesto.html"))
            .Replace("{Identificador}", carteira.IdInvestidor)
            .Replace("{Nome}", cliente.NomeAcionista)
            //.Replace("{Endereco}", cliente.Endereco)
            //.Replace("{Email}", cliente.Email)
            //.Replace("{Telefone}", cliente.Telefone)
            .Replace("{QuantidadeTotal}", cliente.DireitoSubscricao.ToString())
            .Replace("{QuantidadeManifestada}", carteira.QuantidadeIntegralizada.ToString())
            .Replace("{QuantidadeManifestadaValor}", carteira.ValorTotal.ToString())
            ;

        var documento = pdfGenerator.Generate(html, cancellationToken);

        string qrCodeBase64 = Convert.ToBase64String(documento);
        return Ok(qrCodeBase64);

        //var fileContentResult = new FileContentResult(documento, "application/octet-stream")
        //{
        //    FileDownloadName = $"Manifesto_{cliente.NomeAcionista}.pdf"
        //};
        //
        //return fileContentResult;
    }

    private CarteiraModel CriarModelo(CarteiraEntity carteira, ClienteEntity? cliente = null) => new(this, carteira, cliente);
}
