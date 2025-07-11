using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Nastice.GoogleAuthenticateLab.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthCheckController : ControllerBase
    {
        private readonly DbContext _context;
        private readonly ILogger<HealthCheckController> _logger;

        public HealthCheckController(DbContext context, ILogger<HealthCheckController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("Database")]
        [ProducesResponseType(typeof(SimpleResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> DbHealthCheckAsync()
        {
            var request = new
            {
                OrderNo = "2025060617220325",
                Price = 35999.2m,
                ProductName = "Apple iPhone 13 Pro Max"
            };

            _logger.LogWarning("訂單資訊：{@Order}", request);

            var result = await _context.Database.CanConnectAsync();
            if (!result)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "Database connection failed.");
            }

            return Ok(new SimpleResult());
        }

        public sealed class SimpleResult
        {
            public SimpleResult(string? result = "Ok")
            {
                Result = result;
            }

            public string? Result { get; }
        }
    }
}
