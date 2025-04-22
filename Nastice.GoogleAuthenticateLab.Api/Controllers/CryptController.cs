using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nastice.GoogleAuthenticateLab.Services.Libraries;

namespace Nastice.GoogleAuthenticateLab.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CryptController : ControllerBase
    {
        private readonly AesSecurityLibrary _aesSecurityLibrary;

        public CryptController(AesSecurityLibrary aesSecurityLibrary)
        {
            _aesSecurityLibrary = aesSecurityLibrary;
        }

        [HttpPost("AesEncrypt")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public IActionResult Encrypt(string data)
        {
            var encrypted = _aesSecurityLibrary.Encrypt(data);
            return Ok(encrypted);
        }

        [HttpPost("AesDecrypt")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public IActionResult Decrypt(string data)
        {
            var decrypted = _aesSecurityLibrary.Decrypt(data);
            return Ok(decrypted);
        }
    }
}
