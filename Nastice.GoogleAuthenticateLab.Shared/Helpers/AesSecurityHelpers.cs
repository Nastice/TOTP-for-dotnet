using System.Security.Cryptography;

namespace Nastice.GoogleAuthenticateLab.Shared.Helpers;

public static class AesSecurityHelpers
{
    public static byte[] EncryptAsBytesArray(byte[] hashBytes, byte[] key, byte[] iv)
    {
        using var aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var transform = aes.CreateEncryptor();
        var output = transform.TransformFinalBlock(hashBytes, 0, hashBytes.Length);

        aes.Clear();

        return output;
    }

    public static byte[] DecryptAsBytesArray(byte[] hashBytes, byte[] key, byte[] iv)
    {
        using var aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var transform = aes.CreateDecryptor();
        var output = transform.TransformFinalBlock(hashBytes, 0, hashBytes.Length);

        aes.Clear();

        return output;
    }
}
