using desafioPonta.Core.Common.Exceptions;
using desafioPonta.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace desafioPonta.Filters;

public class ApiExceptionFilter : ExceptionFilterAttribute
{
    const string Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
    private readonly ILogger<ApiExceptionFilter> _logger;

    public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger)
    {
        _logger = logger;
    }

    public override void OnException(ExceptionContext context)
    {
        context.HttpContext.Items.Add(nameof(Exception), context.Exception);

        ErrorModel errorModel;
        switch (context.Exception)
        {
            case RulesException ex:
                ServicePointManager.Expect100Continue = false;
                errorModel = new ErrorModel
                {
                    Status = 417,
                    Title = ex.Message,
                    Type = Type,
                    Messages = ex.Messages.GroupBy(x => x.Member)
                        .ToDictionary(x => x.Key, x => x.Select(y => y.Message).Where(z => z != null).ToArray())
                };
                _logger.LogError(ex, $"Fluxo: ApiExceptionFilter, Erro {errorModel.Status}: {string.Join(",", ex.Messages.ToArray())} Trace: {context.Exception.StackTrace} Exception: {context.Exception}");
                break;
            case UnauthorizedAccessException ua:
                errorModel = new ErrorModel
                {
                    Status = 417,
                    Title = ua.Message,
                    Type = Type
                };
                _logger.LogError(ua, $"Fluxo: ApiExceptionFilter, Erro {errorModel.Status}: {errorModel.Messages} Trace: {context.Exception.StackTrace} Exception: {context.Exception}");
                break;
            default:
                {
                    // Unhandled errors
                    var msg = context.Exception.GetBaseException().Message;
                    errorModel = new ErrorModel
                    {
                        Status = 500,
                        Title = msg,
                        Type = Type
                    };
                    _logger.LogError($"Fluxo: ApiExceptionFilter, Erro {errorModel.Status}: {errorModel.Messages} Trace: {context.Exception.StackTrace} Exception: {context.Exception}");
                    break;
                }
        }

#if DEBUG
        errorModel.Stack = context.Exception.StackTrace;
#endif

        // always return a JSON result
        context.HttpContext.Response.StatusCode = errorModel.Status;
        context.Result = new JsonResult(errorModel);
        base.OnException(context);
    }
}