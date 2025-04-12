using System.Diagnostics;
using Castle.DynamicProxy;
using Nastice.GoogleAuthenticateLab.Shared.Extensions;
using Nastice.GoogleAuthenticateLab.Shared.Resources;
using Serilog;

namespace Nastice.GoogleAuthenticateLab.Shared.Interceptors;

public class TraceLogInterceptor : IInterceptor
{
    private readonly ILogger _logger;

    public TraceLogInterceptor(ILogger logger)
    {
        _logger = logger.ForContext<TraceLogInterceptor>();
    }

    public void Intercept(IInvocation invocation)
    {
        var stopwatch = new Stopwatch();
        // 方法執行前
        _logger.Verbose(
        LogMessages.Shared.Interceptors.TraceStart,
        invocation.TargetType,
        invocation.Method.Name,
        getMethodParameters(invocation).ToJson()
        );

        stopwatch.Start();

        try
        {
            // 執行原始方法
            invocation.Proceed();

            // 方法執行後
            _logger.Verbose(
            LogMessages.Shared.Interceptors.TraceEnd,
            invocation.TargetType,
            invocation.Method.Name,
            invocation.ReturnValue);
        }
        catch (Exception ex)
        {
            // 捕捉例外狀況
            _logger.Error(
            ex,
            LogMessages.Shared.Interceptors.Exception,
            invocation.TargetType,
            invocation.Method.Name,
            ex
            );
            Console.WriteLine(ex);
        }
        finally
        {
            stopwatch.Stop();

            var elapsed = stopwatch.ElapsedMilliseconds;
            if (elapsed > 5000)
            {
                _logger.Warning(
                LogMessages.Shared.Interceptors.TakeTooLong,
                invocation.TargetType,
                invocation.Method.Name,
                elapsed);
            }
        }
    }

    private static Dictionary<string, object> getMethodParameters(IInvocation invocation)
    {
        var parameters = new Dictionary<string, object>();

        // 取得參數名稱和對應值
        var methodParams = invocation.Method.GetParameters();
        for (var i = 0; i < methodParams.Length; i++)
        {
            var paramName = methodParams[i].Name;
            var paramValue = invocation.Arguments[i];
            parameters.Add(paramName ?? methodParams[i].ToString(), paramValue ?? string.Empty);
        }

        return parameters;
    }
}