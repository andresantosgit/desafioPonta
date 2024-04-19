//-------------------------------------------------------------------------------------			
// <copyright file="ManifestoController.cs" company="BNB">
//    Copyright statement. All right reserved
// </copyright>	
// <summary>
//   Descrição do arquivo
// </summary>		
//-------------------------------------------------------------------------------------

using BNB.ProjetoReferencia.WebUI.Filters;
using BNB.ProjetoReferencia.Core.Domain.Util;
using BNB.ProjetoReferencia.WebUI.ViewModel.Views.Manifesto;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using BNB.ProjetoReferencia.Core.Domain.ExternalServices.Entities;
using BNB.ProjetoReferencia.Core.Domain.ExternalServices.Interfaces;

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
        private IAuthService _authService;


        /// <summary>
        /// Inicia uma nova instância da classe <see cref="ManifestoController"/> 
        /// </summary>
        /// <param name="logger">Name = "Logger Service"</param>
        public ManifestoController(ILogger<ManifestoController> logger, IAuthService authService)
        {
            _logger = logger;
            _authService = authService;
        }

        /// <summary>
        /// Index view
        /// </summary>
        /// <param name="sucesso">Name = "sucesso"</param>
        /// <returns>Retorna view</returns>
        public async Task<ActionResult> Index(bool? sucesso)
        {
            try
            {
                //ViewBag.SucessoInsercao = this.TempData["SucessoInsercao"];
                //if (sucesso != null)
                //{
                //    ViewBag.SucessoInsercao = sucesso;
                //}
                //else
                //{
                //    ViewBag.TempError = this.TempData["TempError"];
                //    ViewBag.SucessoInsercao = this.TempData["SucessoInsercao"];
                //}

                return this.View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro de exceção.");
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                
                //Response.StatusDescription(ex.Message);
                return this.View();
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

        /// <summary>
        /// Ddetalhes GET
        /// </summary>
        /// <param name="codigo">Name = "codigo"</param>
        /// <returns>Retorna view</returns>
        //public ActionResult Detalhes(int codigo)
        //{
        //    ViewBag.fromDenuncia = true;
        //    var lobjDenuncia = _unitOfWork.DenunciaAuditoriaService.Consultar(codigo);
        //    var viewModel = Mapper.Map<DenunciaAuditoria, DenunciaAuditoriaViewModel>(lobjDenuncia);

        //    return this.View(viewModel);
        //}

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

                return this.RedirectToAction("Erro","Home");
            }
        }

        /// <summary>
        /// Consultar dados.
        /// </summary>
        /// <param name="filtros">Dados de filtro de pesquisa.</param>
        /// <returns>Partial View com a Tabela de Denúncias</returns>
        [HttpPost]
        [Validate]
        public ActionResult ConsultarDados(string cpfCnpj)
        {
            var newModel = new ManifestoNewViewModel();
            newModel.MatriculaSolicitante = _authService.Matricula;
            newModel.CPFOuCNPJ = cpfCnpj;

            newModel.NomeInvestidor = "Camillaaaaaaaaa";
            newModel.Endereco = "Rua Nunes Valente, 2390 AP 402";
            newModel.Email = "camillacgas@gmail.com";
            newModel.Telefone = "(85) 997673062";
            newModel.TipoPessoa = 2;
            newModel.TipoCustodia = 1;
            newModel.ValorAcao = 33.33m;

            return this.JsonDeny(newModel);
        }

        /// <summary>
        /// Registrar POST
        /// </summary>
        /// <param name="viewModel">Name = "aobjDenunciaAuditoria"</param>
        /// <returns>Retorna view</returns>
        [HttpPost]
        [Validate]
        //[ValidateInput(false)]
        public async Task<ActionResult> Registrar(ManifestoNewViewModel viewModel)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            try
            {
                //        var usuarioLogado = Colaborador;

                //        if (ModelState.IsValid && viewModel != null)
                //        {
                //            var model = Mapper.Map<RegistrarViewModel, DenunciaAuditoria>(viewModel);

                //            await this.FixViewModelForRegistrar(model, viewModel, usuarioLogado);

                //            if (viewModel.DataRecebimentoDenuncia == null && !viewModel.DenunciaTerceiro)
                //            {
                //                viewModel.DataRecebimentoDenuncia = DateTime.Now;
                //            }

                //            if (viewModel.DemandanteAuditoriaCodigo != 0)
                //            {
                //                model.Demandante = _unitOfWork.DemandanteAuditoriaService.Consultar(viewModel.DemandanteAuditoriaCodigo);
                //            }

                //            model.Origem = "Intranet";

                //            _unitOfWork.DenunciaAuditoriaService.CriarOuAtualizar(model);

                //            this.SalvaEnvolvidosManuaisDenuncia(model, viewModel);

                //            var parametro = _unitOfWork.ConstantesEVariaveisService.ObterConstantesEVariaveisPeloNome(ConstPrazoAtendimentoDenunciaEmDias).FirstOrDefault() ?? throw new Exception("Parâmetro não encontrado: "+ ConstPrazoAtendimentoDenunciaEmDias);
                //            int prazoAtendimentoDenunciaEmDias = int.Parse(parametro.Valor);

                //            DemandaAuditoria lobjDemandaAuditoria = this.MontaObjetoDemandaAuditoria(model, usuarioLogado);

                //            lobjDemandaAuditoria.UsuarioCadastro = lobjDemandaAuditoria.ResponsavelEncaminhamento = lobjDemandaAuditoria.UsuarioAtualizacao;
                //            lobjDemandaAuditoria.NumeroAnoDemanda = DateTime.Now.Year;

                //            //Número sequencial da Demanda
                //            lobjDemandaAuditoria.SequencialAnoDemanda = _unitOfWork.DemandaAuditoriaService.RetonarUltimoNumeroSequencialAnoDemanda();
                //            lobjDemandaAuditoria.Historico = string.Empty;
                //            var versaoQuestionario = _unitOfWork.VersaoQuestionarioService.ObterQuestionarioAtivoPorTipo(TipoQuestionario.Demanda);
                //            lobjDemandaAuditoria.VersaoQuestionario = versaoQuestionario;

                //            _unitOfWork.DemandaAuditoriaService.CriarOuAtualizar(lobjDemandaAuditoria);
                //            model.DemandaAuditoria = lobjDemandaAuditoria;

                //            if (model.DataRecebimentoDenuncia == null)
                //            {
                //                model.DataRecebimentoDenuncia = DateTime.Now;
                //            }

                //            _unitOfWork.DenunciaAuditoriaService.CriarOuAtualizar(model);


                //            this.TempData["SucessoInsercao"] = true;
                //            ViewBag.NumeroControle = model.NumeroControle;

                //            return this.View("_NumeroControle");
                //        }

                //        if (viewModel != null)
                //        {
                //            this.EspecificarDenunciaToView(viewModel.TipoDenuncia.ToString(), viewModel.DenunciaTerceiro);
                //        }

                //        this.CarregarDemandantesAtivos();
                this.CarregarTiposPessoas();
                this.CarregarTiposCustodias();
                //        this.TempData["SucessoInsercao"] = false;
                return this.View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro de exceção.");

                this.TempData["SucessoInsercao"] = false;
                this.TempData["TempError"] = string.Format("Message: {0}\nStackTrace: {1}", ex.Message, ex.StackTrace);
                return this.RedirectToAction("Index");
            }
        }

            /// <summary>
            /// Função secundária para manipular Registrar(RegistrarViewModel viewModel), visando a redução de complexidade do Sonar.
            /// </summary>
            /// <param name="model">Model DenunciaAuditoria</param>
            /// <param name="usuarioLogado">ColaboradorBNB usuarioLogado</param>
            /// <returns>Retorna o objeto de Demanda Auditoria.</returns>
            //private DemandaAuditoria MontaObjetoDemandaAuditoria(DenunciaAuditoria model, ICredencial usuarioLogado)
            //{
            //    var lobjDemandaAuditoria = new DemandaAuditoria()
            //    {
            //        //Texto = string.Format("Realizar análise de conveniência de apuração de irregularidade e, se for o caso, trabalho de auditoria, na modalidade Exame de Admissibilidade, tomando por base o teor da denúncia Nº {0}.", model.Codigo),
            //        Texto = string.Format(_unitOfWork.ConstantesEVariaveisService.ObterConstantesEVariaveisPeloNome(Constantes.TextoDemandaDenuncia).FirstOrDefault().Valor, model.Codigo),
            //        Demandante = model.Demandante,

            //        //CanalDemanda = this.canalDemandaAuditoriaService.GetByCodigo(int.Parse(ConfigurationManager.AppSettings["CanalDenunciaCodigo"])),
            //        CanalDemanda = _unitOfWork.CanalDemandaAuditoriaService.Consultar(
            //                    int.Parse(_unitOfWork.ConstantesEVariaveisService.ObterConstantesEVariaveisPeloNome(ConstCanalDenuncia)
            //                               .Select(d => d.Valor)
            //                               .FirstOrDefault())),
            //        //TipoDemanda = this.tipoDemandaAuditoriaService.GetByCodigo(int.Parse(ConfigurationManager.AppSettings["TipoDemandaAuditoriaIrregularidadeCodigo"])),
            //        TipoDemanda = _unitOfWork.TipoDemandaAuditoriaService.Consultar(
            //                    int.Parse(_unitOfWork.ConstantesEVariaveisService.ObterConstantesEVariaveisPeloNome(ConstDemandaApuracaoIrregularidades)
            //                               .Select(d => d.Valor)
            //                               .FirstOrDefault())),
            //        DataAtendimento = DateTime.Now.AddDays(
            //                    int.Parse(_unitOfWork.ConstantesEVariaveisService.ObterConstantesEVariaveisPeloNome(ConstPrazoAtendimentoDenunciaEmDias)
            //                        .Select(d => d.Valor)
            //                        .FirstOrDefault())).Date,
            //        //Status = StatusDemandaAuditoria.AguardandoAnalise,
            //        Status = _unitOfWork.StatusFluxoService.Listar().Where(x => x.ModuloSistema.Nome == Constantes.NomeModuloDemanda && x.Nome == Constantes.DescricaoStatusAguardandoAnalise).FirstOrDefault().Codigo,
            //        DataRecebimentoBNB = model.DataRecebimentoDenuncia == null ? DateTime.Now.Date : model.DataRecebimentoDenuncia,
            //        DataRecebimentoAuditoria = DateTime.Now.Date,
            //        NotaPrioridade = 0,
            //        DataCadastro = DateTime.Now,
            //        DataAtualizacao = DateTime.Now,
            //        UsuarioAtualizacao = (model.TipoDenuncia == TipoDenuncia.Anonima && model.DenunciaTerceiro == false) ? "S493" : usuarioLogado.Matricula + " - " + usuarioLogado.Nome.ToString().Trim(),
            //    };

            //    return lobjDemandaAuditoria;
            //}

            /// <summary>
            /// Função FixViewModelForRegistrar
            /// </summary>
            /// <param name="model">Dados Denúncia Auditoria</param>
            /// <param name="viewModel">Dados de registro da denúncia.</param>
            /// <param name="usuarioLogado">Dados do usuário logado.</param>
            //private async Task FixViewModelForRegistrar(DenunciaAuditoria model, RegistrarViewModel viewModel, ICredencial usuarioLogado)
            //{
            //    model.Anexos = model.Anexos ?? new List<Anexos>();
            //    model.ColaboradoresEnvolvidos = model.ColaboradoresEnvolvidos ?? new List<ColaboradorBNB>();
            //    model.UnidadesEnvolvidas = model.UnidadesEnvolvidas ?? new List<UnidadeBNB>();
            //    model.NumeroControle = ExtensionMethods.GenerateRandom();
            //    var statusDenunciaAguardandoAnaliseDaAuditoria = _unitOfWork.StatusDenunciaService.Listar(x => x.DescricaoStatusDenuncia == Domain.Enums.StatusDenuncia.AguardandoAnalise.ObterDescricao()).FirstOrDefault();
            //    model.Status = statusDenunciaAguardandoAnaliseDaAuditoria;

            //    if ((model.TipoDenuncia == TipoDenuncia.Anonima && model.DenunciaTerceiro) || model.TipoDenuncia == TipoDenuncia.Identificada)
            //    {
            //        model = await this.FixViewModelForRegistrarModelManipulacao(model, usuarioLogado);
            //    }

            //    if (viewModel.UnidadesEnvolvidasCodigos != null)
            //    {
            //        var unidadesEnvolvidas = new List<UnidadeBNB>();

            //        foreach (var i in viewModel.UnidadesEnvolvidasCodigos)
            //        {
            //            if (i != 0)
            //            {
            //                unidadesEnvolvidas.Add(_unitOfWork.UnidadeBNBService.Consultar(i));
            //            }
            //        }

            //        model.UnidadesEnvolvidas = unidadesEnvolvidas;
            //    }

            //    if (viewModel.ColaboradoresEnvolvidosMatriculas != null)
            //    {
            //        var colaboradores = new List<ColaboradorBNB>();

            //        foreach (var i in viewModel.ColaboradoresEnvolvidosMatriculas)
            //        {
            //            if (!string.IsNullOrEmpty(i))
            //            {
            //                colaboradores.Add(_unitOfWork.ColaboradorBNBService.RetornarEntidadeColaboradorPorMatricula(i));
            //            }
            //        }

            //        model.ColaboradoresEnvolvidos = colaboradores;
            //    }
            //}

            /// <summary>
            /// Função secundária para manipular FixViewModelForRegistrar, visando a redução de complexidade
            /// </summary>
            /// <param name="model">Dados Denúncia Auditoria</param>
            /// <param name="usuarioLogado">Dados do usuário logado.</param>
            /// <returns>Retorna a model de DenunciaAuditoria</returns>
            //private async Task<DenunciaAuditoria> FixViewModelForRegistrarModelManipulacao(DenunciaAuditoria model, ICredencial usuarioLogado)
            //{
            //    //if (this.Credencial.perfil.Any(x => x.Descricao.ToUpper() == "COMISSÃO DE ÉTICA" || x.Descricao.ToUpper() == "OUVIDORIA" || x.Descricao.ToUpper() == "COMITÊ DE AUDITORIA"))
            //    if ((await this.ObterListaPerfis()).Any(x => x.Descricao.ToUpper() == "COMISSÃO DE ÉTICA" || x.Descricao.ToUpper() == "OUVIDORIA" || x.Descricao.ToUpper() == "COMITÊ DE AUDITORIA"))
            //    {
            //        //model.ResponsavelCadastroUnidade = this.Credencial.perfil.Any(x => x.Descricao.ToUpper() == "COMISSÃO DE ÉTICA") ? "COMISSÃO DE ÉTICA" : this.Credencial.perfil.Any(x => x.Descricao.ToUpper() == "OUVIDORIA") ? "OUVIDORIA" : "COMITÊ DE AUDITORIA";
            //        model.ResponsavelCadastroUnidade = (await this.ObterListaPerfis()).Any(x => x.Descricao.ToUpper() == "COMISSÃO DE ÉTICA") ? "COMISSÃO DE ÉTICA" : (await this.ObterListaPerfis()).Any(x => x.Descricao.ToUpper() == "OUVIDORIA") ? "OUVIDORIA" : "COMITÊ DE AUDITORIA";
            //    }
            //    else
            //    {
            //        var lobjUsuarioLogado = _unitOfWork.ColaboradorBNBService.ObterColaboradorPelaMatricula(usuarioLogado.Matricula);

            //        if (lobjUsuarioLogado != null)
            //        {
            //            model.ResponsavelCadastroUnidade = lobjUsuarioLogado.NomeLotacaoReal.Trim();
            //        }                
            //    }

            //    model.ResponsavelCadastro = new ColaboradorBNB { Matricula = usuarioLogado.Matricula, Nome = usuarioLogado.Nome };
            //    model.IPResponsavelCadastro = ViewModel.Util.Extensoes.ClienteIP(HttpContext)["IdentidadeIpOrigem"]; 

            //    if (string.IsNullOrEmpty(model.NomeDenunciante) && !string.IsNullOrEmpty(model.MatriculaDenunciante))
            //    {
            //        var lobjColaborador = _unitOfWork.ColaboradorBNBService.ObterColaboradorPelaMatricula(model.MatriculaDenunciante);

            //        if (lobjColaborador != null)
            //        {
            //            model.NomeDenunciante = _unitOfWork.ColaboradorBNBService.ObterColaboradorPelaMatricula(model.MatriculaDenunciante).Nome;
            //        }
            //    }

            //    return model;
            //}

            /// <summary>
            /// Salva envolvidos Manuais Denúncia
            /// </summary>
            /// <param name="model">Model da Denúncia</param>
            /// <param name="viewModel">View Model da Denúncia</param>
            //private void SalvaEnvolvidosManuaisDenuncia(DenunciaAuditoria model, RegistrarViewModel viewModel)
            //{
            //    if(viewModel.EnvolvidosManuais == null)
            //    {
            //        return;
            //    }

            //    foreach (var envolvidoVM in viewModel.EnvolvidosManuais)
            //    {
            //        if (!string.IsNullOrEmpty(envolvidoVM))
            //        {
            //            var dadosEnvolvido = envolvidoVM.Split(',');
            //            var envolvido = new EnvolvidoDenuncia()
            //            {
            //                Nome = dadosEnvolvido[0],
            //                Funcao = _unitOfWork.FuncaoEnvolvidoService.Consultar(int.Parse(dadosEnvolvido[1])),
            //                DescricaoFuncao = dadosEnvolvido[2],
            //                Denuncia = model
            //            };
            //            _unitOfWork.EnvolvidoDenunciaService.Criar(envolvido);
            //        }
            //    }
            //}

            /// <summary>
            /// Validar request
            /// </summary>
            /// <returns>Retorna um JSON com a Propriedade Success com valor true se a VM for validada com sucesso.</returns>
            [HttpPost]
        //public JsonResult ValidarRequest()
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            return this.Json(new { Success = true });
        //        }
        //        else
        //        {
        //            var errorModel = from x in ModelState.Keys
        //                             where ModelState[x].Errors.Count > 0
        //                             select new ErroValidacao
        //                             {
        //                                 Propriedade = x,
        //                                 Erros = ModelState[x]
        //                                     .Errors
        //                                     .Select(y => y.ErrorMessage)
        //                                     .ToList()
        //                             };
        //            return this.Json(errorModel);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Erro de exceção.");

        //        return this.Json(new ErroValidacao
        //        {
        //            Propriedade = "Erro Interno de Servidor",
        //            Erros = new List<string> { "Ocorreu um erro no processamento da requisição. Tente novamente mais tarde." }
        //        });
        //    }
        //}

        /// <summary>
        /// Acompanhamento de denuncia GET
        /// </summary>
        /// <returns>Retorna view</returns>
        public ActionResult AcompanhamentoDenuncia()
        {
            ViewBag.NotFound = this.TempData["NotFound"] ?? false;
            return this.View();
        }

        /// <summary>
        /// Acompanhamento POST
        /// </summary>
        /// <param name="numeroControle">Name = "numeroControle"</param>
        /// <returns>Retorna view</returns>
        [HttpPost]
        //public ActionResult AcompanhamentoDenuncia(string numeroControle)
        //{
        //    PartialDenunciaAcompanharDenunciaViewModel partialModel = new PartialDenunciaAcompanharDenunciaViewModel();
        //    DenunciaAuditoria denuncia = _unitOfWork.DenunciaAuditoriaService.ObterPorNumeroControle(numeroControle);

        //    if (denuncia != null && denuncia.DemandaAuditoria != null)
        //    {
        //        partialModel = Mapper.Map<DenunciaAuditoria, PartialDenunciaAcompanharDenunciaViewModel>(denuncia);
        //        if (denuncia.DemandaAuditoria.StatusFluxo != null && StringFormatter.removerCaracteresEpeciais(denuncia.DemandaAuditoria.StatusFluxo.Nome).Equals(StringFormatter.removerCaracteresEpeciais(Constantes.StatusDemandaArquivada)))
        //        {
        //            var descricaoJustificativa = _unitOfWork.JustificativaDemandaService.Listar()
        //                .Where(x => x.DemandaAuditoria.Codigo == denuncia.DemandaAuditoria.Codigo
        //                    && StringFormatter.removerCaracteresEpeciais(x.StatusFluxo.Nome).Equals(StringFormatter.removerCaracteresEpeciais(Constantes.StatusDemandaEmApreciacaoDeArquivamento)))
        //                .Select(x => x.CodigoMotivo.Descricao)
        //                .FirstOrDefault();

        //            partialModel.DemandaAuditoriaMotivoArquivamentoDescricao = descricaoJustificativa ?? "";
        //        }
        //    }

        //    return this.PartialView("_DenunciaAcompanharDenuncia", partialModel);

        //    //return this.PartialView(
        //    //    "_DenunciaAcompanharDenuncia",
        //    //    Mapper.Map<DenunciaAuditoria, PartialDenunciaAcompanharDenunciaViewModel>(this.denunciaAuditoriaService.ObterPorNumeroControle(numeroControle)));
        //}

        /// <summary>
        /// Consulta de Denúncias
        /// </summary>
        /// <returns>Retorna View</returns>
        public ActionResult ConsultaDenuncias()
        {
            ViewBag.Error = TempData["Error"] ?? false;
            ViewBag.NotFound = TempData["NotFound"] ?? false;
            return this.View();
        }

        /// <summary>
        /// Listar Denuncias.
        /// </summary>
        /// <param name="filtros">Dados de filtro de pesquisa.</param>
        /// <returns>Partial View com a Tabela de Denúncias</returns>
        //[HttpPost]
        //[Validate]
        //public ActionResult ListarDenuncias(DenunciaAuditoriaFiltroViewModel filtros)
        //{
        //    if (filtros != null && filtros.DataCadastroFinal.HasValue)
        //    {
        //        filtros.DataCadastroFinal = filtros.DataCadastroFinal.Value.AddHours(23);
        //        filtros.DataCadastroFinal = filtros.DataCadastroFinal.Value.AddMinutes(59);
        //        filtros.DataCadastroFinal = filtros.DataCadastroFinal.Value.AddSeconds(59);
        //    }

        //    var denuncias = _unitOfWork.DenunciaAuditoriaService.FiltrarDenuncias(Mapper.Map<DenunciaAuditoriaFiltroViewModel, DenunciaAuditoriaServiceFiltro>(filtros));

        //    //JsonResult lobjJson = this.JsonDeny(new List<PartialListaDenunciasViewModel>());
        //    var partialDenuncia = Mapper.Map<List<DenunciaAuditoria>, List<PartialListaDenunciasViewModel>>(denuncias.ToList());
            
        //    return this.JsonDeny(partialDenuncia);
        //}

        /// <summary>
        /// Visualizar Denúncia
        /// </summary>
        /// <param name="codigo">Código da Denúncia pesquisada</param>
        /// <returns>View com os dados da denúncia</returns>
        //public async Task<IActionResult> VisualizarDenuncia(int? codigo)
        //{
        //    try
        //    {
        //        VisualizarDenunciaViewModel partialModel = new VisualizarDenunciaViewModel();

        //        if (codigo == null)
        //        {
        //            TempData["NotFound"] = true;
        //            return this.RedirectToAction("ConsultaDenuncias");
        //        }

        //        var denuncia = _unitOfWork.DenunciaAuditoriaService.Consultar((int)codigo);
        //        if (denuncia == null)
        //        {
        //            TempData["NotFound"] = true;
        //            return this.RedirectToAction("ConsultaDenuncias");
        //        }

        //        //Registra Trilha de Auditoria na pesquisa de Denuncia            
        //        TrilhaTO lobjTrilhaTO = this.obterDadosTrilhaAuditoria();
        //        lobjTrilhaTO.DescricaoFuncionalidadeEvento = "Consulta Denuncia";
        //        lobjTrilhaTO.DescricaoInformacoesAdicionais = "Nº Denúncia: " + denuncia.Codigo;
        //        lobjTrilhaTO.IdentidadeTipoEvento = TipoEventoTrilhaAuditoria.Consulta.GetHashCode().ToString();
        //        //var auditado = await Auditar(lobjTrilhaTO);
        //        //_logger.LogInformation(lobjTrilhaTO.ToString());
        //        _trilhaAuditoriaService.Auditar(lobjTrilhaTO);

        //        partialModel = Mapper.Map<DenunciaAuditoria, VisualizarDenunciaViewModel>(denuncia);
        //        if (denuncia.DemandaAuditoria != null && StringFormatter.removerCaracteresEpeciais(denuncia.DemandaAuditoria.StatusFluxo.Nome).Equals(StringFormatter.removerCaracteresEpeciais(Constantes.StatusDemandaArquivada)))
        //        {

        //            string descricaoJustificativa = "";

        //            try
        //            {
        //                descricaoJustificativa = _unitOfWork.JustificativaDemandaService.Listar()
        //                    .Where(x => x.DemandaAuditoria.Codigo == denuncia.DemandaAuditoria.Codigo
        //                        && StringFormatter.removerCaracteresEpeciais(x.StatusFluxo.Nome).Equals(StringFormatter.removerCaracteresEpeciais(Constantes.StatusDemandaEmApreciacaoDeArquivamento)))
        //                    .Select(x => x.CodigoMotivo.Descricao)
        //                    .FirstOrDefault();
        //            }
        //            catch (Exception ex)
        //            {
        //                descricaoJustificativa = "";
        //            }

        //            partialModel.DemandaAuditoriaMotivoArquivamentoDescricao = descricaoJustificativa;
        //        }

        //        return this.View(partialModel);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Erro de exceção.");

        //        TempData["Error"] = true;
        //        return this.RedirectToAction("ConsultaDenuncias");
        //    }
        //}

        /// <summary>
        /// Obtem colaboradores envolvidos GET
        /// </summary>
        /// <param name="query">Name = "query"</param>
        /// <returns>Retorna JSON</returns>
        //[HttpGet]
        //public JsonResult ObterColaboradoresEnvolvidos(string query)
        //{
        //    dynamic resultado = new
        //    {
        //        success = true,
        //        results = _unitOfWork.ColaboradorBNBService.FiltrarColaboradoresBNB(query)
        //            .Select(c => new
        //            {
        //                name = c.Matricula + " - " + c.Nome.Trim(),
        //                text = c.Matricula + " - " + c.Nome.Trim(),
        //                value = c.Matricula
        //            })
        //            .ToList()
        //    };
        //    return this.Json(resultado);
        //}

        /// <summary>
        /// Obter Colaborador pela Matrícula
        /// </summary>
        /// <param name="matricula">Matrícula do colaborador BNB</param>
        /// <returns>JSON com colaborador</returns>
        //[HttpGet]
        //public JsonResult ObterColaboradorPorMatricula(string matricula)
        //{
        //    if (matricula != null)
        //    {
        //        return this.Json(_unitOfWork.ColaboradorBNBService.RetornarEntidadeColaboradorPorMatricula(matricula));
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        /// <summary>
        /// Obtem os registradores de denúncias a partir de um filtro
        /// </summary>
        /// <param name="query">Name = "query"</param>
        /// <returns>Retorna JSON</returns>
        //[HttpGet]
        //public JsonResult ObterRegistradores(string query)
        //{
        //    dynamic resultado = new
        //    {
        //        success = true,
        //        results = _unitOfWork.DenunciaAuditoriaService.ObterRegistradores(query)
        //            .Select(c => new
        //            {
        //                name = c.Matricula + " - " + c.Nome,
        //                text = c.Matricula + " - " + c.Nome,
        //                value = c.Matricula
        //            })
        //            .ToList()
        //    };
        //    return this.Json(resultado);
        //}      
    }
}