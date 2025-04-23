using System.Text;
using Microsoft.Extensions.Options;
using Nastice.GoogleAuthenticateLab.Services.Interfaces;
using Nastice.GoogleAuthenticateLab.Shared.Extensions;
using Nastice.GoogleAuthenticateLab.Shared.Helpers;
using Nastice.GoogleAuthenticateLab.Shared.Models.Options;

namespace Nastice.GoogleAuthenticateLab.Services.Libraries;

public class AesSecurityLibrary : ISecurityLibrary
{
    private readonly AesOptions _options;

    public AesSecurityLibrary(IOptions<AesOptions> options)
    {
        _options = options.Value;
    }

    public string Encrypt(string value)
    {
        var bytesValue = Encoding.UTF8.GetBytes(value);
        var bytesKey = _options.Key.HexToByteArray();
        var bytesIv = _options.Iv.HexToByteArray();

        var encryptedBytes = AesSecurityHelpers.EncryptAsBytesArray(bytesValue, bytesKey, bytesIv);

        var encrypted = Convert.ToHexString(encryptedBytes);

        return encrypted;
    }

    public string Decrypt(string value)
    {
        var bytesValue = value.HexToByteArray();
        var bytesKey = _options.Key.HexToByteArray();
        var bytesIv = _options.Iv.HexToByteArray();

        var decryptedBytes = AesSecurityHelpers.DecryptAsBytesArray(bytesValue, bytesKey, bytesIv);

        var decrypted = Encoding.UTF8.GetString(decryptedBytes);

        return decrypted;
    }
}
