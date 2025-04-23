namespace Nastice.GoogleAuthenticateLab.Services.Interfaces;

public interface ISecurityLibrary
{
    /// <summary>
    /// 加密
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public string Encrypt(string value);

    /// <summary>
    /// 解密
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public string Decrypt(string value);
}
