using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nastice.GoogleAuthenticateLab.Shared.Exceptions;
using Nastice.GoogleAuthenticateLab.Shared.Resources;

namespace Nastice.GoogleAuthenticateLab.Shared.Filters;

public class ExceptionFilter : IAsyncExceptionFilter, IExceptionFilter
{
    private readonly IHostEnvironment _hostEnv;
    private readonly ILogger<ExceptionFilter> _logger;

    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
        WriteIndented = true
    };

    public ExceptionFilter(IHostEnvironment hostEnv, ILogger<ExceptionFilter> logger)
    {
        _hostEnv = hostEnv;
        _logger = logger;
    }
    public async Task OnExceptionAsync(ExceptionContext context)
    {
        handleException(context);
        await Task.CompletedTask;
    }

    public void OnException(ExceptionContext context)
    {
        handleException(context);
    }

    #region Private Methods

    private void handleException(ExceptionContext context)
    {
        var statusCode = StatusCodes.Status500InternalServerError;

        _logger.LogError(LogMessages.Shared.Filters.ExceptionFilter.OccurredException,
            context.Exception.Message,
            context.Exception.StackTrace,
            context.Exception.GetType().FullName,
            context
        );

        var response = new ProblemDetails
        {
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
            Title = "系統忙碌中，請稍後嘗試",
            Status = statusCode
        };

        if (_hostEnv.IsDevelopment() || _hostEnv.IsStaging())
        {
            response.Detail
                = $"Exception occurred in dev mode. Error Message: {context.Exception.Message}, Stack trace: {context.Exception.StackTrace}";
            response.Extensions.Add("stackTrace", context.Exception.StackTrace?.Split("\r\n"));
        }

        switch (context.Exception)
        {
            case BadHttpRequestException badRequestException:
                handleHttpBadRequestException(response, badRequestException);
                statusCode = badRequestException.StatusCode;
                break;
            case CommonException commonException:
                handleCommonException(response, commonException);
                statusCode = commonException.HttpStatus;
                break;
        }

        context.Result = new ContentResult
        {
            Content = JsonSerializer.Serialize(response, _jsonSerializerOptions),
            StatusCode = statusCode
        };
    }

    private static void handleHttpBadRequestException(ProblemDetails problem, BadHttpRequestException badRequestException)
    {
        problem.Type = $"https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/{badRequestException.StatusCode}";
        problem.Title = badRequestException.Message;
        problem.Extensions.Add("content", badRequestException.Message);
        problem.Status = badRequestException.StatusCode;
    }

    private static void handleCommonException(ProblemDetails problem, CommonException commonException)
    {
        problem.Type = $"https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/{commonException.HttpStatus}";
        problem.Title = commonException.Message;
        problem.Extensions.Add("content", commonException.Message);
        problem.Status = commonException.HttpStatus;
    }

    #endregion
}
