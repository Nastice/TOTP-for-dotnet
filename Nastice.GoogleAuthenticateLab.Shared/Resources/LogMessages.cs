namespace Nastice.GoogleAuthenticateLab.Shared.Resources;

public static class LogMessages
{
    public static class Shared
    {
        public static class Filters
        {
            public const string TraceStart = "Entering route {Uri}. Executing action {ControllerName}@{ActionName} with parameters: {@Params}";
            public const string TraceEnd = "Completed route {Uri}. Response from {ControllerName}@{ActionName} with result: {@Returns}";
        }

        public static class Interceptors
        {
            public const string TraceStart = "Method {Class}@{Method} started with arguments: {@Arguments}";

            public const string TraceEnd = "Method {Class}@{Method} ended with result: {@Results}";

            public const string Exception = "Method {Class}@{Method} threw an exception: {@Exception}";

            public const string TakeTooLong = "Method {Class}@{Method} execution took too long: {ElapsedMilliseconds}ms.";
        }
    }
}
