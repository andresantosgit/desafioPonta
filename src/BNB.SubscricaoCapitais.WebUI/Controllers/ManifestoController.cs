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
using BNB.ProjetoReferencia.Infrastructure.Database.SQLite.Repositories;
using BNB.ProjetoReferencia.WebUI.Filters;
using BNB.ProjetoReferencia.WebUI.Helpers.Erros;
using BNB.ProjetoReferencia.WebUI.ViewModel.Views.Manifesto;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading;

namespace BNB.ProjetoReferencia.WebUI.Controllers
{
    /// <summary>
    /// Controller de manifesto
    /// </summary>
    public class ManifestoController : BaseController
    {
        /// <summary>
        /// Logger Service
        /// </summary>
        private readonly ILogger<ManifestoController> _logger;
        private readonly IAuthService _authService;
        private readonly IClienteRepository _clienteRepository;
        private readonly ICarteiraRepository _carteiraRepository;
        private readonly IRequestHandler<DomainEvent<CriarCarteiraEvent>, CarteiraEntity> _criarCarteiraEventHandler;
        private readonly IRequestHandler<DomainEvent<AtualizarClienteEvent>, ClienteEntity> _atualizarClienteEventHandler;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ManifestoController"/> 
        /// </summary>
        /// <param name="logger">Name = "Logger Service"</param>
        public ManifestoController(ILogger<ManifestoController> logger,
                                   IAuthService authService,
                                   IClienteRepository clienteRepository,
                                   ICarteiraRepository carteiraRepository,
                                   IRequestHandler<DomainEvent<CriarCarteiraEvent>, CarteiraEntity> criarCarteiraEventHandler,
                                   IRequestHandler<DomainEvent<AtualizarClienteEvent>, ClienteEntity> atualizarClienteEventHandler,
                                   IConfiguration configuration)
        {
            _logger = logger;
            _authService = authService;
            _clienteRepository = clienteRepository;
            _carteiraRepository = carteiraRepository;
            _criarCarteiraEventHandler = criarCarteiraEventHandler;
            _atualizarClienteEventHandler = atualizarClienteEventHandler;
            _configuration = configuration;
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

            try
            {
                var eventoCriarCarteira = new DomainEvent<CriarCarteiraEvent>(new(viewModel.CPFOuCNPJ, viewModel.Quantidade.Value, viewModel.MatriculaSolicitante));
                //var eventoAtualizarCliente = new DomainEvent<AtualizarClienteEvent>(new(viewModel.CPFOuCNPJ, viewModel.Endereco, viewModel.Telefone, viewModel.Email, viewModel.MatriculaSolicitante));

                //var clienteAtualizado = await _atualizarClienteEventHandler.Handle(eventoAtualizarCliente, cancellationToken);
                var novaCarteira = await _criarCarteiraEventHandler.Handle(eventoCriarCarteira, cancellationToken);

                ViewBag.SucessoInsercao = true;
                return this.View(viewModel);
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
                return this.View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro de exceção.");
                ViewBag.SucessoInsercao = false;
            #if DEBUG
                ViewBag.TempError = string.Format("Message: {0}\nStackTrace: {1}", ex.Message, ex.StackTrace);
            #endif
                return this.View(viewModel);
            }
        }

        /// <summary>
        /// Registrar GET
        /// </summary>
        /// <returns>Retorna view</returns>
        public ActionResult Consultar()
        {
            try
            {
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

            cpfCnpj = Uri.UnescapeDataString(cpfCnpj);
            var carteiras = await _carteiraRepository.FindAllByIdInvestidorAsync(cpfCnpj, cancellationToken);

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

            JsonResult lobjJson = this.Json(list);
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
                return BadRequest();

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

        private async Task<List<ManifestoNewViewModel>> BuscarHistoricoInvestidor(string cpfCnpj, CancellationToken cancellationToken)
        {
            var list = new List<ManifestoNewViewModel>();
            //for (int i = 1; i <= 3; i++)
            //{
            //    var dados = BuscaDadosInvestidor(cpfCnpj);
            //    dados.Id = i;

            //    list.Add(dados);
            //}
            return list;
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

    }
}