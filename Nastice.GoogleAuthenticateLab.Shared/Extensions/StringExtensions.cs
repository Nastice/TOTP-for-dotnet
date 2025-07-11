using System.Globalization;
using QRCoder;

namespace Nastice.GoogleAuthenticateLab.Shared.Extensions;

public static class StringExtensions
{
    public static byte[] HexToByteArray(this string hexString)
    {
        var byteLength = hexString.Length / 2;
        var bytes = new byte[byteLength];

        for (int i = 0, j = 0; i < bytes.Length; i++, j += 2)
        {
            var hex = new string(new[] { hexString[j], hexString[j + 1] });
            bytes[i] = hexToByte(hex);
        }

        return bytes;
    }

    private static byte hexToByte(string hex)
    {
        if (hex.Length is > 2 or <= 0)
        {
            throw new ArgumentException("hex must be 1 or 2 characters in length");
        }

        var newByte = byte.Parse(hex, NumberStyles.HexNumber);
        return newByte;
    }

    public static string ToQrcode(this string obj)
    {
        var imageBytes = PngByteQRCodeHelper.GetQRCode(obj, QRCodeGenerator.ECCLevel.L, 6);

        var imageString = Convert.ToBase64String(imageBytes);
        return imageString;
    }
}
