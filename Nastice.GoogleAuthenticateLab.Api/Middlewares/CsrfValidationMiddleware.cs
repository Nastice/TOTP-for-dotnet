using System.Net;
using System.Net.Mime;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Caching.Distributed;
using Nastice.GoogleAuthenticateLab.Services.Interfaces;
using Nastice.GoogleAuthenticateLab.Services.Libraries;
using Nastice.GoogleAuthenticateLab.Shared;
using Nastice.GoogleAuthenticateLab.Shared.Constants;
using Nastice.GoogleAuthenticateLab.Shared.Enums;
using Nastice.GoogleAuthenticateLab.Shared.Extensions;
using Nastice.GoogleAuthenticateLab.Shared.Resources;

namespace Nastice.GoogleAuthenticateLab.Middlewares;

public class CsrfValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IDistributedCache _distributedCache;
    private readonly ILogger<CsrfValidationMiddleware> _logger;

    public CsrfValidationMiddleware(RequestDelegate next, ILogger<CsrfValidationMiddleware> logger, IDistributedCache distributedCache)
    {
        _next = next;
        _logger = logger;
        _distributedCache = distributedCache;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (requiresCsrfValidation(context) && await isCsrfTokenValid(context) != CsrfResultCode.Success)
        {
            _logger.LogError("CSRF Token is not valid.");

            var problemDetails = createProblemDetails(HttpStatusCode.Forbidden, context.TraceIdentifier, "CSRF 令牌異常或已失效，請重新登入！");

            await responseErrorAsync(context, HttpStatusCode.Forbidden, problemDetails);
            return;
        }

        await _next(context);
    }

    #region Private Method

    /// <summary>
    /// 驗證 Controller Action 是否需要經過 CSRF 檢核
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    private bool requiresCsrfValidation(HttpContext context)
    {
        var method = context.Request.Method;

        var endpoint = context.GetEndpoint();

        var endpointMetadata = endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();
        if (endpointMetadata == null)
        {
            _logger.LogDebug(LogMessages.Api.Middlewares.CsrfValidationMiddleware.NoControllerActionDescriptorFound);
            return false;
        }

        var classAttributes = endpointMetadata.ControllerTypeInfo.GetCustomAttributes(true);
        var methodAttributes = endpointMetadata.MethodInfo.GetCustomAttributes(true);

        var classRequireAntiForgeryToken = classAttributes.Any(x => x is RequireCsrfTokenAttribute);
        var methodRequireAntiForgeryToken = methodAttributes.Any(x => x is RequireCsrfTokenAttribute);

        if (!classRequireAntiForgeryToken && !methodRequireAntiForgeryToken)
        {
            _logger.LogDebug(LogMessages.Api.Middlewares.CsrfValidationMiddleware.NoRequireAntiforgeryTokenAttribute);
            return false;
        }

        var result = HttpMethods.IsPost(method) || HttpMethods.IsPut(method) || HttpMethods.IsDelete(method);

        _logger.LogDebug(LogMessages.Api.Middlewares.CsrfValidationMiddleware.RequireValidateCsrfToken,
            endpointMetadata.ControllerTypeInfo.FullName,
            endpointMetadata.MethodInfo.Name,
            result ? "Yes" : "No");

        return HttpMethods.IsPost(method) || HttpMethods.IsPut(method) || HttpMethods.IsDelete(method);
    }

    /// <summary>
    /// 檢核 CSRF 令牌是否有效
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    private async Task<CsrfResultCode> isCsrfTokenValid(HttpContext context)
    {
        var securityLibrary = context.RequestServices.GetRequiredKeyedService<ISecurityLibrary>(nameof(AesSecurityLibrary));

        var csrfToken = context.Request.Headers["X-CSRF-Token"]
            .ToString();
        if (string.IsNullOrEmpty(csrfToken))
        {
            _logger.LogWarning(LogMessages.Api.Middlewares.CsrfValidationMiddleware.CsrfTokenVerifyFailed, CsrfResultCode.EmptyCsrfToken.ToString());
            return CsrfResultCode.EmptyCsrfToken;
        }

        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)
            ?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning(LogMessages.Api.Middlewares.CsrfValidationMiddleware.CsrfTokenVerifyFailed,
                CsrfResultCode.CouldNotFoundUser.ToString());
            return CsrfResultCode.CouldNotFoundUser;
        }

        var rawToken = await _distributedCache.GetStringAsync($"{RedisKeys.CsrfToken}:{userId}");
        if (string.IsNullOrEmpty(rawToken))
        {
            _logger.LogWarning(LogMessages.Api.Middlewares.CsrfValidationMiddleware.CsrfTokenVerifyFailed,
                CsrfResultCode.StoredCsrfTokenNotFound.ToString());
            return CsrfResultCode.StoredCsrfTokenNotFound;
        }

        var token = securityLibrary.Decrypt(rawToken);
        if (csrfToken != token)
        {
            _logger.LogWarning(LogMessages.Api.Middlewares.CsrfValidationMiddleware.CsrfTokenVerifyFailed, CsrfResultCode.CsrfNotMatch.ToString());
            return CsrfResultCode.CsrfNotMatch;
        }

        return CsrfResultCode.Success;
    }

    /// <summary>
    /// 生成錯誤資訊
    /// </summary>
    /// <param name="status"></param>
    /// <param name="traceId"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    private static ProblemDetails createProblemDetails(HttpStatusCode status, string traceId, string message)
    {
        var result = new ProblemDetails
        {
            Type = $"https://developer.mozilla.org/zh-TW/docs/Web/HTTP/Reference/Status/{(int)status}",
            Title = message,
            Status = (int)status
        };

        result.Extensions.Add("traceId", traceId);

        return result;
    }

    private async Task responseErrorAsync(HttpContext context, HttpStatusCode statusCode, object response)
    {
        var responseJson = response.ToJson();
        if (response is ProblemDetails problemDetails)
        {
            _logger.LogError(LogMessages.Api.Middlewares.CsrfValidationMiddleware.CsrfTokenVerifyFailed, problemDetails.Title);
        }

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = MediaTypeNames.Application.ProblemJson;

        await context.Response.WriteAsync(responseJson);
        await context.Response.Body.FlushAsync();
    }

    #endregion
}
