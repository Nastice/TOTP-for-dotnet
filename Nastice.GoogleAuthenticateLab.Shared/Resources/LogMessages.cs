namespace Nastice.GoogleAuthenticateLab.Shared.Resources;

public static class LogMessages
{
    public static class Api
    {
        public static class Controllers
        {
            public static class LoginController
            {
                public const string UserNotFound = "User account {Account} not found.";

                public const string Unauthorized = "{Account} attempted to log in but failed due to {Reason}.";
            }

            public static class RegisterController
            {
                public const string UserCreatedFailed = "Could not create user account {Account}.";
            }
        }
    }

    public static class Services
    {
        public static class BaseService
        {
            public const string CreateAffectRows = "Create entity {Entity} affected {AffectRows} rows";
            public const string AffectedNoRows = "Save {Entity} affected no rows.";
        }

        public static class UserService
        {
            public const string PasswordIncorrect = "Password for {Account} is incorrect.";
            public const string InvalidOtp = "Invalid OTP for {Account}.";
            public const string OtpIsEmpty = "OTP for {Account} is required but was empty.";
            public const string UserIsNotEnable = "User {Account} is not enabled.";
        }
    }

    public static class Shared
    {
        public static class Filters
        {
            public static class ExceptionFilter
            {
                public const string OccurredException
                    = "Occurred exception! Message: {Message}, StackTrace: {@StackTrace}, ExceptionType: {ExceptionType}, RawException {@Exception}";
            }

            public static class TraceLogFilter
            {
                public const string RouteEntered = "Entering route {Uri}. Executing action {ControllerName}@{ActionName} with parameters: {@Params}";
                public const string RouteExited = "Completed route {Uri}. Response from {ControllerName}@{ActionName} with result: {@Returns}";
            }
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
