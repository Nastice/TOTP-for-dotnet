using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Nastice.GoogleAuthenticateLab.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class ProfileController : ControllerBase
{
    // GET
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Index()
    {
        var user = User;

        await Task.CompletedTask;
        return Ok();
    }
}
