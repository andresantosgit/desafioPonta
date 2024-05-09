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
using Microsoft.AspNetCore.Mvc;
using BNB.ProjetoReferencia.Core.Domain.ExternalServices.Interfaces;
using Microsoft.Extensions.Configuration;

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

    private readonly bool _isISKeyDisabled;

    public SegurancaAutorizaActionFilter(
                ILogger<SegurancaAutorizaActionFilter> logger,
                IAuthService authService,
                IConfiguration configuration)
    {
        _logger = logger;
        _authService = authService;
        _isISKeyDisabled = configuration.GetValue<bool>("ISKey:Disabled"); ;
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

            Debug.WriteLine(string.Format("{0}/{1}", lstrController, lstrAction));

            string matricula =_authService.Matricula;
            string lstrAplicacao = string.Format("{0}/{1}", lstrController, lstrAction);            

            if (this.EhAcessoLiberado(lstrAplicacao) || _authService.HasPermission(lstrAplicacao))
            {
                Debug.WriteLine(string.Format("{0}/{1}: ACESSO AUTORIZADO", lstrAplicacao, matricula));
            }
            else if (! _authService.HasPermission(lstrAplicacao))
            {
                AcessoNegado(filtroContexto, string.Format("{0}/{1}", lstrAplicacao, matricula));                
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
        _logger.LogInformation(string.Format("{0}: ACESSO NEGADO", mensagem));

        var routeValue = new RouteValueDictionary(new { action = "AcessoNegado", controller = "Home" });

        filtroContexto.Result = new RedirectToRouteResult(routeValue);
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
           "Home/Index",        // PAGINA INICIAL DA APLICACAO
           "Home/AcessoNegado", // ACESSO NEGADO
           "Home/Erro", //ERRO
           "Home/Logout" // SAIR
        };

        return _isISKeyDisabled || larrAcessoLiberado.Contains<string>(identificador);
    }
}
