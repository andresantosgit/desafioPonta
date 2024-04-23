//-------------------------------------------------------------------------------------			
// <copyright file="SegurancaActionAttribute.cs" company="BNB">
//    Copyright statement. All right reserved
// </copyright>	
// <summary>
//   Descrição do arquivo
// </summary>		
//-------------------------------------------------------------------------------------
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Controllers;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using BNB.ProjetoReferencia.Core.Domain.ExternalServices.Interfaces;

namespace BNB.ProjetoReferencia.WebUI.Filters;

/// <summary>
/// Segurança autorização
/// </summary>
//[AttributeUsage(AttributeTargets.Class)]
//[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class SegurancaAutorizaActionFilter : ActionFilterAttribute, IAsyncActionFilter   //IActionFilter
{
    private readonly ILogger<SegurancaAutorizaActionFilter> _logger;

    private readonly IAuthService _authService;

    //private readonly IMapper _mapper;

    public SegurancaAutorizaActionFilter(
                ILogger<SegurancaAutorizaActionFilter> logger,
                IAuthService authService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        //_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    /// <summary>
    /// On action executing
    /// </summary>
    /// <param name="filtroContexto">Name = "filtroContexto"</param>
    public override async Task OnActionExecutionAsync(ActionExecutingContext filtroContexto, ActionExecutionDelegate next)
    {
        if (filtroContexto != null)
        {
            string? lstrAction = ((ControllerActionDescriptor)filtroContexto.ActionDescriptor).ActionName;
            string? lstrController = ((ControllerActionDescriptor)filtroContexto.ActionDescriptor).ControllerName;

            if (_authService == null || lstrAction == null || lstrController == null)
            {
                AcessoNegado(filtroContexto, "(Erro na inicialização dos parâmetros de segurança.)");
            }

            string lstrIP = "";
            if (filtroContexto.HttpContext.Connection.RemoteIpAddress != null)
            {
                lstrIP = filtroContexto.HttpContext.Connection.RemoteIpAddress.ToString();
            }

            //string lstrIP = filtroContexto.HttpContext.Request.UserHostAddress;                

            // USAR PARA CADASTRO NO S267-ISKEY
            //Debug.WriteLine(string.Format("{0}:{1}:{2}:{3}", lstrController, lstrAction, lstrIP, ldatDateTime.ToLongDateString()));
            Debug.WriteLine(string.Format("{0}:{1}:{2}", lstrController, lstrAction, lstrIP));

            //SetColaboradorLogado(filtroContexto);
            //var colaborador = _globalVariablesService.GetColaboradorViewModel();

            string matricula =_authService.Matricula;

            //  (colaborador != null) ? colaborador.Matricula : 

            // CHECA SE USUARIO POSSUI PERMISSAO NA APLICACAO (CONTROLLER:ACTION)
            string lstrAplicacao = string.Format("{0}:{1}", lstrController, lstrAction);            

            if (this.EhAcessoLiberado(lstrAplicacao) || _authService.HasPermission(lstrAplicacao))
            {
                Debug.WriteLine(string.Format("{0}:{1}:ACESSO AUTORIZADO", lstrAplicacao, matricula));
            }
            else if (! _authService.HasPermission(lstrAplicacao))
            {
                AcessoNegado(filtroContexto, string.Format("{0}:{1}", lstrAplicacao, matricula));                
            }
            else
            {
                var routeValue = new RouteValueDictionary(new { action = "Index", controller = "Authentication" });
                filtroContexto.Result = new RedirectToRouteResult(routeValue);
            }
        }
        
        if (filtroContexto.Result == null)
        {
            await next(); // the actual action
        }
    }

    

    /// <summary>
    /// Redireciona para view de acesso negado
    /// </summary>
    /// <param name="filtroContexto">Name = "filtroContexto"</param>
    /// <param name="mensagem">Name = "mensagem"</param>
    private void AcessoNegado(ActionExecutingContext filtroContexto, string mensagem)
    {
        _logger.LogInformation(string.Format("{0}:ACESSO NEGADO", mensagem));

        var routeValue = new RouteValueDictionary(new { action = "AcessoNegado", controller = "Home" });

        filtroContexto.Result = new RedirectToRouteResult(routeValue);
        //filtroContexto.Result = new RedirectToRouteResult(
        //        new System.Web.Routing.RouteValueDictionary(new { controller = "Home", action = "AcessoNegado" }));
    }

    /// <summary>
    /// LIBERAR TODAS AS PAGINAS E ACOES DE ACESSO LIVRE NA APLICACAO
    /// PARA AS PAGINAS FARA UM REDIRECIONAMENTO PARA A PAGINA DE ACESSO NEGADO
    /// </summary>
    /// <param name="identificador">Name = "identificador"</param>
    /// <returns>Retorna booleano</returns>
    private bool EhAcessoLiberado(string identificador)
    {
        string[] larrAcessoLiberado = new string[]
        {
           "Home:Index",        // PAGINA INICIAL DA APLICACAO
           "Home:AcessoNegado", // ACESSO NEGADO
           "Home:EmManutencao",  // MANUTENCAO
           "Home:ErroDocumento", //ERRO DOCUMENTO
           "Home:Erro", //ERRO
           "Home:Logout", // SAIR
           "Home:ErroUsuario", // ERRO USUÁRIO
           "DenunciaAuditoria:AcompanhamentoDenuncia",
           "TrabalhoAuditoria:ObterRoteiroAuditoria", // ACTION DE CARREGAMENTO DE COMBO DO TRABALHO
           "TrabalhoAuditoria:ObterModalidadesAuditoria", // ACTION DE CARREGAMENTO DE COMBO DO TRABALHO
           "TrabalhoAuditoria:ObterTiposDeAuditoria", // ACTION DE CARREGAMENTO DE COMBO DO TRABALHO
           "TrabalhoAuditoria:TipoTrabalhoRequerObjetoAuditoria", // ACTION DE CARREGAMENTO DE COMBO DO TRABALHO
           "TrabalhoAuditoria:ObterObjetoAuditoria", // ACTION DE CARREGAMENTO DE COMBO DO TRABALHO
           "TrabalhoAuditoria:ObterUnidadeGestoraAuditoria", // ACTION DE CARREGAMENTO DE COMBO DO TRABALHO
           "TrabalhoAuditoria:ObterExecutorAuditoria", // ACTION DE CARREGAMENTO DE COMBO DO TRABALHO
           "TrabalhoAuditoria:ObterGrupoResponsavelAuditoria", // ACTION DE CARREGAMENTO DE COMBO DO TRABALHO
           "TrabalhoAuditoria:ObterCoordenadorTrabalholAuditoria", // ACTION DE CARREGAMENTO DE COMBO DO TRABALHO
           "TrabalhoAuditoria:ObterModalidadesAuditoriaSituacaoS3" // ACTION DE VALIÇÃO DE CAMPO DO TRABALHO
        };
        //var lstrActionsPublicas = _unitOfWork.ConstantesEVariaveisService.ObterConstantesEVariaveisPeloNome(Constantes.ConstNomeAcoesPublicas).FirstOrDefault().Valor;

        // string[] larrActionsPublicas = lstrActionsPublicas.Split(',');

        return true; // larrAcessoLiberado.Union(larrActionsPublicas).Contains<string>(identificador);
    }
}
