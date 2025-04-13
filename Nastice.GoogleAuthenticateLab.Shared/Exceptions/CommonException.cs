using Microsoft.AspNetCore.Http;

namespace Nastice.GoogleAuthenticateLab.Shared.Exceptions;

public class CommonException : Exception
{
    public int HttpStatus { get; set; }

    public CommonException(string? message, int httpStatus = 500) : base(message)
    {
        HttpStatus = httpStatus == 0 ? StatusCodes.Status500InternalServerError : httpStatus;
    }
}
