using Microsoft.AspNetCore.Mvc;

namespace Nastice.GoogleAuthenticateLab.Controllers;

public class ApiControllerBase : ControllerBase
{
    protected ProblemDetails createProblemDetails(int statusCode, string? title)
    {
        var problemDetails = ProblemDetailsFactory.CreateProblemDetails(HttpContext, statusCode, title);

        return problemDetails;
    }
}
