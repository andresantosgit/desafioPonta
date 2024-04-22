using BNB.ProjetoReferencia.Core.Common.Helper;
using BNB.ProjetoReferencia.Core.Common.Interfaces;
using BNB.ProjetoReferencia.Core.Common.Validations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Internal;

namespace BNB.ProjetoReferencia.Extensions;

public static class ControllerBaseExtensions
{
    public static DomainEvent<TModel> CriarEventoDominio<TModel>(this ControllerBase controller, TModel model)
    {
        // Podemos implementar o esquema de Auditoria aqui, o exemplo esta abaixo
        //return new(model, controller.CriarAuditoriaAsync().Result);
        return new(model);
    }

    public static async Task<Auditoria> CriarAuditoriaAsync(this ControllerBase controller)
    {
        var id = Guid.NewGuid();
        if (!Guid.TryParse(controller.Request.Headers["CorrelationId"], out Guid correlationId))
            correlationId = id;

        var auditoria = new Auditoria
        {
            CriadoEm = controller.HttpContext?.RequestServices.GetService<ISystemClock>()?.UtcNow ?? DateTimeOffset.Now,
            CriadoPor = controller.Request.Headers["CriadoPor"].ToString() ?? string.Empty,
            Id = id,
            Origem = string.IsNullOrWhiteSpace(controller.Request.Headers["App"]) ? "BNBProjetoReferencia" : controller.Request.Headers["App"].ToString(),
            CorrelationId = correlationId
        };

        var validator = new AuditoriaRules();
        (await validator.FactoryAsync(auditoria, new CancellationToken())).Validate();

        return auditoria;
    }

    public static string Link<TController>(this ControllerBase controller, string actionName, object routeValues = null)
        where TController : ControllerBase
    {
        var controllerName = typeof(TController).Name.Replace("Controller", string.Empty);
        var href = controller.Url.ActionLink(actionName, controllerName, routeValues);
        return href;
    }
}
