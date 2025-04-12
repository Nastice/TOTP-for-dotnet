using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nastice.GoogleAuthenticateLab.Shared.Extensions;
using Nastice.GoogleAuthenticateLab.Shared.Resources;

namespace Nastice.GoogleAuthenticateLab.Shared.Filters;

public class TraceLogFilter : IActionFilter, IAsyncActionFilter
{
        public void OnActionExecuting(ActionExecutingContext context)
        {
            logTraceStart(context);
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            logTraceEnd(context);
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            logTraceStart(context);

            var resultContext = await next();

            logTraceEnd(resultContext);
        }

        /// <summary>
        /// 紀錄開始 Log
        /// </summary>
        /// <param name="context"></param>
        private static void logTraceStart(ActionExecutingContext context)
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<TraceLogFilter>>();

            var controllerName = getControllerName(context.HttpContext);
            var methodName = getMethodName(context.HttpContext);

            var uri = context.HttpContext.Request.Path.Value;

            logger.LogTrace(LogMessages.Shared.Filters.TraceStart, uri, controllerName, methodName, context.ActionArguments.ToJson());
        }

        /// <summary>
        /// 紀錄結束 Log
        /// </summary>
        /// <param name="context"></param>
        private static void logTraceEnd(ActionExecutedContext context)
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<TraceLogFilter>>();

            var controllerName = getControllerName(context.HttpContext);
            var methodName = getMethodName(context.HttpContext);

            var uri = context.HttpContext.Request.Path.Value;

            var result = string.Empty;
            if (context.Result is ObjectResult objectResult)
            {
                result = objectResult.Value?.ToJson() ?? string.Empty;
            }

            logger.LogTrace(LogMessages.Shared.Filters.TraceEnd, uri, controllerName, methodName, result);
        }

        /// <summary>
        /// 取得 Controller 名稱
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static string getControllerName(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            var endpointMetadata = endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();
            return endpointMetadata?.ControllerTypeInfo.FullName ?? string.Empty;
        }

        /// <summary>
        /// 取得 Action 名稱
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static string getMethodName(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            var endpointMetadata = endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();
            return endpointMetadata?.MethodInfo.Name ?? string.Empty;
        }
}
