//-------------------------------------------------------------------------------------			
// <copyright file="Validate.cs" company="BNB">
//    Copyright statement. All right reserved
// </copyright>	
// <summary>
//   Descrição do arquivo
// </summary>		
//-------------------------------------------------------------------------------------

namespace BNB.ProjetoReferencia.WebUI.Filters
{
    using System.Net;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Mvc;
    using BNB.ProjetoReferencia.WebUI.Helpers;
    using BNB.ProjetoReferencia.WebUI.Helpers.Erros;


    /// <summary>
    /// Classe validade
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class Validate : ActionFilterAttribute
    {
        /// <summary>
        /// On action executing
        /// </summary>
        /// <param name="context">Name = "filterContext"</param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context != null)
            {
                var controller = (Controller)context.Controller;
                //var modelState = filterContext.Controller.ViewData.ModelState;
                var modelState = controller.ViewData.ModelState;
                if (!modelState.IsValid)
                {
                    var errorModel = from x in modelState.Keys
                                     where modelState[x].Errors.Count > 0
                                     select new ErroValidacao
                                     {
                                         Propriedade = x,
                                         Erros = modelState[x]
                                             .Errors
                                             .Select(y => y.ErrorMessage)
                                             .ToList()
                                     };

                    if (errorModel.Count() > 0)
                    {
                        //if (filterContext.HttpContext.Request.IsAjaxRequest())
                        //if (filterContext.HttpContext.Request.Headers["x-requested-with"] == "XMLHttpRequest")
                        if (AjaxRequestHelper.IsAjaxRequest(context.HttpContext.Request))
                        {
                            //filterContext.Result = new JsonResult() { Data = errorModel.ToList() };
                            context.Result = new JsonResult(new { Data = errorModel.ToList() });
                            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        }
                        else
                        {
                            controller.ViewBag.Erros = errorModel.ToList();
                            base.OnActionExecuting(context);
                        }
                    }
                }
            }
        }

    }
}