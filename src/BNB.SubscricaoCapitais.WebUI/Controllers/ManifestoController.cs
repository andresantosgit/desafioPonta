//-------------------------------------------------------------------------------------			
// <copyright file="ManifestoController.cs" company="BNB">
//    Copyright statement. All right reserved
// </copyright>	
// <summary>
//   Descrição do arquivo
// </summary>		
//-------------------------------------------------------------------------------------

using BNB.ProjetoReferencia.Core.Common.Exceptions;
using BNB.ProjetoReferencia.Core.Common.Interfaces;
using BNB.ProjetoReferencia.Core.Domain.Carteira.Entities;
using BNB.ProjetoReferencia.Core.Domain.Carteira.Events;
using BNB.ProjetoReferencia.Core.Domain.Carteira.Interfaces;
using BNB.ProjetoReferencia.Core.Domain.Cliente.Entities;
using BNB.ProjetoReferencia.Core.Domain.Cliente.Events;
using BNB.ProjetoReferencia.Core.Domain.Cliente.Interfaces;
using BNB.ProjetoReferencia.Core.Domain.ExternalServices.Interfaces;
using BNB.ProjetoReferencia.Core.Domain.Util;
using BNB.ProjetoReferencia.WebUI.Filters;
using BNB.ProjetoReferencia.WebUI.Helpers.Erros;
using BNB.ProjetoReferencia.WebUI.ViewModel.Views.Manifesto;
using BNB.SubscricaoCapitais.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using QRCoder;
using System.Text;

namespace BNB.ProjetoReferencia.WebUI.Controllers
{
    /// <summary>
    /// Controller de manifesto
    /// </summary>
    public class ManifestoController : BaseController
    {
        private readonly ILogger<ManifestoController> _logger;
        private readonly IClienteRepository _clienteRepository;
        private readonly ICarteiraRepository _carteiraRepository;
        private readonly IRequestHandler<DomainEvent<CriarCarteiraEvent>, CarteiraEntity> _criarCarteiraEventHandler;
        private readonly IRequestHandler<DomainEvent<AtualizarClienteEvent>, ClienteEntity> _atualizarClienteEventHandler;
        private readonly IConfiguration _configuration;
        private readonly IPDFGenerator _pdfGenerator;
        private readonly IAuthService _authService;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ManifestoController"/> 
        /// </summary>
        public ManifestoController(ILogger<ManifestoController> logger,
                                   IClienteRepository clienteRepository,
                                   ICarteiraRepository carteiraRepository,
                                   IRequestHandler<DomainEvent<CriarCarteiraEvent>, CarteiraEntity> criarCarteiraEventHandler,
                                   IRequestHandler<DomainEvent<AtualizarClienteEvent>, ClienteEntity> atualizarClienteEventHandler,
                                   IConfiguration configuration,
                                   IPDFGenerator pdfGenerator, 
                                   IAuthService authService)
        {
            _logger = logger;
            _clienteRepository = clienteRepository;
            _carteiraRepository = carteiraRepository;
            _criarCarteiraEventHandler = criarCarteiraEventHandler;
            _atualizarClienteEventHandler = atualizarClienteEventHandler;
            _configuration = configuration;
            _pdfGenerator = pdfGenerator;
            _authService = authService;
        }

        /// <summary>
        /// Registrar GET
        /// </summary>
        /// <returns>Retorna view</returns>
        public ActionResult Registrar()
        {
            try
            {
                this.CarregarTiposPessoas();
                this.CarregarTiposCustodias();

                var newModel = new ManifestoNewViewModel();
                newModel.MatriculaSolicitante = _authService.Matricula;

                ViewBag.Colaborador = _authService.GetCredencial();

                return this.View(newModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro de exceção.");

                return this.RedirectToAction("Erro", "Home");
            }
        }

        /// <summary>
        /// Registrar POST
        /// </summary>
        /// <param name="viewModel">Name = "aobjDenunciaAuditoria"</param>
        /// <returns>Retorna view</returns>
        [HttpPost]
        [Validate]
        public async Task<ActionResult> Registrar(ManifestoNewViewModel viewModel, CancellationToken cancellationToken)
        {
            if (viewModel == null)
                throw new ArgumentNullException(nameof(viewModel));

            this.CarregarTiposPessoas();
            this.CarregarTiposCustodias();

            ViewBag.Colaborador = _authService.GetCredencial();

            try
            {
                var eventoCriarCarteira = new DomainEvent<CriarCarteiraEvent>(new(viewModel.CPFOuCNPJ, viewModel.Quantidade.Value, viewModel.MatriculaSolicitante));
                var novaCarteira = await _criarCarteiraEventHandler.Handle(eventoCriarCarteira, cancellationToken);

                viewModel.Id = novaCarteira.Id;
                ViewBag.SucessoInsercao = true;
                return this.Json(viewModel);
            }
            catch (RulesException ex)
            {
                var rulesError = ex.Messages
                    .GroupBy(x => x.Member)
                    .ToDictionary(x => x.Key, x => x.Select(y => y.Message).Where(z => z != null).ToArray())
                    .Select(x => new ErroValidacao()
                    {
                        Propriedade = x.Key,
                        Erros = x.Value
                    });
                ViewBag.SucessoInsercao = false;
                ViewBag.Erros = rulesError.ToList();
                return this.RedirectToAction("Erro", "Home");
                //return this.View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro de exceção.");
                ViewBag.SucessoInsercao = false;
#if DEBUG
                ViewBag.TempError = string.Format("Message: {0}\nStackTrace: {1}", ex.Message, ex.StackTrace);
#endif
                return this.RedirectToAction("Erro", "Home");
                //return this.View(viewModel);
            }
        }

        /// <summary>
        /// Consultar
        /// </summary>
        /// <returns>Retorna view</returns>
        public ActionResult Consultar()
        {
            try
            {
                ViewBag.Colaborador = _authService.GetCredencial();
                return this.View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro de exceção.");
                return this.RedirectToAction("Erro", "Home");
            }
        }

        public async Task<ActionResult> ListaManifestos(string cpfCnpj, CancellationToken cancellationToken)
        {
            var list = new List<ManifestoNewViewModel>();
            List<CarteiraEntity> carteiras;

            if (cpfCnpj.IsNullOrEmpty())
            {
                carteiras = await _carteiraRepository.FindAllAsync(cancellationToken);
            }
            else
            {
                cpfCnpj = Uri.UnescapeDataString(cpfCnpj);
                carteiras = await _carteiraRepository.FindAllByIdInvestidorAsync(cpfCnpj, cancellationToken);
            }

            var statusSaldo = _configuration["StatusSaldo"]!;
            var quantidadeAtual = carteiras.Where(x => statusSaldo.Split(";").Contains(x.Status)).Sum(y => y.QuantidadeIntegralizada);

            var clientesIdsInvestidor = carteiras
            .Select(x => x.IdInvestidor)
            .Distinct()
            .Select(x => _clienteRepository.FindByIdInvestidorAsync(x, cancellationToken).Result)
            .ToArray();

            foreach (var carteira in carteiras)
            {
                var cliente = clientesIdsInvestidor.FirstOrDefault(y => y.IdInvestidor == carteira.IdInvestidor);

                var newModel = new ManifestoNewViewModel();
                newModel.Id = carteira.Id;
                newModel.MatriculaSolicitante = carteira.Matricula;
                newModel.CPFOuCNPJ = carteira.IdInvestidor;
                newModel.NomeInvestidor = cliente.NomeAcionista;
                newModel.TipoPessoa = cliente.TipoPessoa == "Física" ? 1 : 2;
                newModel.ValorAcao = carteira.ValorUnitarioPorAcao;
                newModel.QuantidadeMaxima = cliente.DireitoSubscricao - quantidadeAtual;
                newModel.Status = carteira.Status;
                newModel.DataCriacao = carteira.DataCriacao;
                newModel.DataAtualizacao = carteira.DataAtualizacao;
                newModel.Quantidade = carteira.QuantidadeIntegralizada;
                newModel.ValorTotal = carteira.ValorTotal;
                newModel.PixCopiaECola = carteira.PixCopiaECola;

                list.Add(newModel);
            }

            var oderedList = list.OrderByDescending(o => o.DataCriacao);

            JsonResult lobjJson = this.Json(oderedList);
            return lobjJson;
        }

        /// <summary>
        /// Consultar dados.
        /// </summary>
        /// <param name="cpfCnpj">Dados de filtro de pesquisa.</param>
        /// <returns>Partial View com a Tabela de Denúncias</returns>
        [HttpPost]
        [Validate]
        public async Task<ActionResult> ConsultarDados(string cpfCnpj, CancellationToken cancellationToken)
        {
            cpfCnpj = Uri.UnescapeDataString(cpfCnpj);
            var cliente = await _clienteRepository.FindByIdInvestidorAsync(cpfCnpj, cancellationToken);
            if (cliente is null)
            {
                var model = new ManifestoNewViewModel();
                model.CPFOuCNPJ = cpfCnpj;
                this.TempData["Success"] = false;
                return NotFound(model);
            }

            var carteiras = await _carteiraRepository.FindAllByIdInvestidorAsync(cpfCnpj, cancellationToken);
            var statusSaldo = _configuration["StatusSaldo"]!;
            var quantidadeAtual = carteiras.Where(x => statusSaldo.Split(";").Contains(x.Status)).Sum(y => y.QuantidadeIntegralizada);

            var newModel = new ManifestoNewViewModel();
            newModel.MatriculaSolicitante = _authService.Matricula;
            newModel.CPFOuCNPJ = cpfCnpj;

            newModel.NomeInvestidor = cliente.NomeAcionista;
            newModel.Endereco = cliente.Endereco;
            newModel.Email = cliente.Email;
            newModel.Telefone = cliente.Telefone;
            newModel.TipoPessoa = cliente.TipoPessoa == "Física" ? 1 : 2;
            newModel.TipoCustodia = cliente.TipoCustodia == "1-CUSTÓDIA PRÓPRIA" ? 1 : 0;
            newModel.ValorAcao = cliente.ValorUnitarioPorAcao;
            newModel.QuantidadeMaxima = cliente.DireitoSubscricao - quantidadeAtual;

            return this.JsonDeny(newModel);
        }

        [HttpPost]
        [Validate]
        public async Task<ActionResult> AtualizarCliente(ManifestoNewViewModel viewModel, CancellationToken cancellationToken)
        {
            try
            {
                var eventoAtualizarCliente = new DomainEvent<AtualizarClienteEvent>(new(viewModel.CPFOuCNPJ, viewModel.Endereco, viewModel.Telefone, viewModel.Email, viewModel.MatriculaSolicitante));
                var clienteAtualizado = await _atualizarClienteEventHandler.Handle(eventoAtualizarCliente, cancellationToken);

                this.TempData["Success"] = true;
                return this.JsonDeny(viewModel);
            }
            catch (RulesException ex)
            {
                var rulesError = ex.Messages
                    .GroupBy(x => x.Member)
                    .ToDictionary(x => x.Key, x => x.Select(y => y.Message).Where(z => z != null).ToArray())
                    .Select(x => new ErroValidacao()
                    {
                        Propriedade = x.Key,
                        Erros = x.Value
                    });
                this.TempData["Success"] = false;
                ViewBag.Erros = rulesError.ToList();
                return this.JsonDeny(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro de exceção.");
                this.TempData["Success"] = false;
#if DEBUG
                ViewBag.TempError = string.Format("Message: {0}\nStackTrace: {1}", ex.Message, ex.StackTrace);
#endif
                return this.JsonDeny(viewModel);
            }
        }

        private ManifestoNewViewModel BuscaDadosInvestidor(string cpfCnpj)
        {
            var newModel = new ManifestoNewViewModel();

            newModel.MatriculaSolicitante = _authService.Matricula;
            newModel.CPFOuCNPJ = cpfCnpj;
            newModel.NomeInvestidor = "Camilla Gonçalves Amaro de Souza";
            newModel.Endereco = "Rua Nunes Valente, 2390 AP 402";
            newModel.Email = "camillacgas@gmail.com";
            newModel.Telefone = "(85) 997673062";
            newModel.TipoPessoa = 2;
            newModel.TipoCustodia = 1;
            newModel.Quantidade = 3;
            newModel.ValorAcao = 33.33m;
            newModel.ValorTotal = 99.99m;
            newModel.MatriculaSolicitante = "D003373";
            newModel.DataCriacao = DateTime.Now;
            newModel.DataAtualizacao = DateTime.Now;
            newModel.Status = "CONCLUÍDA";

            return newModel;
        }

        /// <summary>
        /// Carrega os dados do PIX
        /// </summary>
        /// <param name="codigo">Codigo</param>
        /// <returns>Json com dados do PIX</returns>
        public JsonResult GerarPix(int codigo)
        {
            var carteira = _carteiraRepository.GetById(codigo);
            if (carteira == null)
                return this.Json(new { qrCodeBase64 = "" });

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(carteira.PixCopiaECola, QRCodeGenerator.ECCLevel.Q);
            BitmapByteQRCode qrCode = new BitmapByteQRCode(qrCodeData);
            string qrCodeBase64 = Convert.ToBase64String(qrCode.GetGraphic(10));

            return this.Json(new { qrCodeBase64 = qrCodeBase64, pixCopiaCola = carteira.PixCopiaECola });
        }

        public async Task<ActionResult> GerarDocumento(int codigo, CancellationToken cancellationToken)
        {
            var carteira = _carteiraRepository.GetById(codigo);
            if (carteira is null)
                return NoContent();

            var cliente = await _clienteRepository.FindByIdInvestidorAsync(carteira.IdInvestidor, cancellationToken);
            if (cliente is null)
                return NoContent();

            var html = (await System.IO.File.ReadAllTextAsync("./manifesto.html"))
                .Replace("{Identificador}", carteira.IdInvestidor)
                .Replace("{Nome}", cliente.NomeAcionista)
                .Replace("{Endereco}", cliente.Endereco)
                .Replace("{Email}", cliente.Email)
                .Replace("{Telefone}", cliente.Telefone)
                .Replace("{QuantidadeTotal}", cliente.DireitoSubscricao.ToString())
                .Replace("{QuantidadeManifestada}", carteira.QuantidadeIntegralizada.ToString())
            .Replace("{QuantidadeManifestadaValor}", carteira.ValorTotal.ToString())
            ;

            var documento = _pdfGenerator.Generate(html, cancellationToken);
            var fileContentResult = new FileContentResult(documento, "application/octet-stream")
            {
                FileDownloadName = $"{carteira.Id.ToString("D4")}_Manifesto_{cliente.NomeAcionista}.pdf"
            };
            return fileContentResult;
        }

        public async Task<ActionResult> GerarRelatorioClienteCSV(string cliente, string? status, CancellationToken cancellationToken)
        {
            var carteiras = await _carteiraRepository.FindAllByIdInvestidorAsync(cliente, cancellationToken);
            if (!carteiras.Any())
                return NoContent();

            if (!string.IsNullOrWhiteSpace(status))
                carteiras = carteiras.Where(x => x.Status.Equals(status, StringComparison.InvariantCultureIgnoreCase)).ToList();

            var clientesIdsInvestidor = carteiras
                .Select(x => x.IdInvestidor)
                .Distinct()
                .Select(x => _clienteRepository.FindByIdInvestidorAsync(x, cancellationToken).Result)
                .ToArray();

            var carteiraClientes = carteiras.Select(x => CriarModelo(x, clientesIdsInvestidor.FirstOrDefault(y => y.IdInvestidor == x.IdInvestidor))).ToList();

            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
            {
                streamWriter.WriteLine($"{nameof(CarteiraModel.Id)};{nameof(CarteiraModel.IdInvestidor)};{nameof(CarteiraModel.NomeAcionista)};{nameof(CarteiraModel.TipoPessoa)};{nameof(CarteiraModel.TxId)};{nameof(CarteiraModel.DataCriacao)};{nameof(CarteiraModel.DataAtualizacao)};{nameof(CarteiraModel.QuantidadeIntegralizada)};{nameof(CarteiraModel.ValorUnitarioPorAcao)};{nameof(CarteiraModel.ValorTotal)};{nameof(CarteiraModel.Status)};{nameof(CarteiraModel.PixCopiaECola)}");
                foreach (var carteira in carteiraClientes)
                    streamWriter.WriteLine($"{carteira.Id};{carteira.IdInvestidor};{carteira.NomeAcionista};{carteira.TipoPessoa};{carteira.TxId};{carteira.DataCriacao};{carteira.DataAtualizacao};{carteira.QuantidadeIntegralizada};{carteira.ValorUnitarioPorAcao};{carteira.ValorTotal};{carteira.Status};{carteira.PixCopiaECola}");

                streamWriter.Flush();
                memoryStream.Position = 0;
                var fileContentResult = new FileContentResult(memoryStream.ToArray(), "application/octet-stream")
                {
                    FileDownloadName = $"RelatorioGlobal.csv"
                };
                return fileContentResult;
            }
        }


        private void CarregarTiposPessoas()
        {
            var tiposPessoas = new List<TipoPessoa>();
            tiposPessoas.Add(new TipoPessoa { Codigo = "", Descricao = "" });
            tiposPessoas.Add(new TipoPessoa { Codigo = "1", Descricao = "Física" });
            tiposPessoas.Add(new TipoPessoa { Codigo = "2", Descricao = "Jurídica" });

            this.ViewData["TiposPessoas"] = tiposPessoas;
        }

        private void CarregarTiposCustodias()
        {
            var tiposCustodias = new List<TipoCustodia>();
            tiposCustodias.Add(new TipoCustodia { Codigo = "1", Descricao = "Custódia Própria" });

            this.ViewData["TiposCustodias"] = tiposCustodias;
        }

        private CarteiraModel CriarModelo(CarteiraEntity carteira, ClienteEntity? cliente = null) => new(this, carteira, cliente);

    }
}