using Microsoft.Extensions.Options;
using Nastice.GoogleAuthenticateLab.Data.Interfaces;
using Nastice.GoogleAuthenticateLab.Data.Nastice.GoogleAuthenticateLab.Data.DTOs;
using Nastice.GoogleAuthenticateLab.Services.Interfaces;
using Nastice.GoogleAuthenticateLab.Shared.Models.Options;
using OtpNet;

namespace Nastice.GoogleAuthenticateLab.Services.Services;

public class TotpService : ITotpService
{
    private readonly IRepositoryBase<User> _userRepository;
    private readonly TotpOptions _totpOptions;

    public TotpService(IRepositoryBase<User> userRepository, IOptions<TotpOptions> totpOptions)
    {
        _userRepository = userRepository;
        _totpOptions = totpOptions.Value;
    }

    public OtpUri GenerateTotpUri(User user, string secret)
    {
        var uri = new OtpUri(OtpType.Totp, secret, user.Email, _totpOptions.Issuer);

        return uri;
    }
}
