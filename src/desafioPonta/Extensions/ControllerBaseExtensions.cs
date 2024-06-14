using desafioPonta.Core.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Internal;

namespace desafioPonta.Extensions;

public static class ControllerBaseExtensions
{
    public static DomainEvent<TModel> CriarEventoDominio<TModel>(this ControllerBase controller, TModel model)
    {
        return new(model);
    }    
    public static string Link<TController>(this ControllerBase controller, string actionName, object routeValues = null)
        where TController : ControllerBase
    {
        var controllerName = typeof(TController).Name.Replace("Controller", string.Empty);
        var href = controller.Url.ActionLink(actionName, controllerName, routeValues);
        return href;
    }
}
