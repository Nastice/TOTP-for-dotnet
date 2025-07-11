using OtpNet;

namespace Nastice.GoogleAuthenticateLab.Shared.Helpers;

public static class TotpHelpers
{
    public static string GenerateTotpSecret(int length = 20)
    {
        var secret = Base32Encoding.ToString(KeyGeneration.GenerateRandomKey(length));

        return secret;
    }
}
