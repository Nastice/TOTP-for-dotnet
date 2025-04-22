using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Nastice.GoogleAuthenticateLab.Data.Interfaces;
using Nastice.GoogleAuthenticateLab.Data.Nastice.GoogleAuthenticateLab.Data.DTOs;
using Nastice.GoogleAuthenticateLab.Services.Interfaces;
using Nastice.GoogleAuthenticateLab.Services.Libraries;
using Nastice.GoogleAuthenticateLab.Shared.Enums;
using Nastice.GoogleAuthenticateLab.Shared.Exceptions;
using Nastice.GoogleAuthenticateLab.Shared.Models;
using Nastice.GoogleAuthenticateLab.Shared.Models.Requests;
using Nastice.GoogleAuthenticateLab.Shared.Resources;

namespace Nastice.GoogleAuthenticateLab.Services.Services;

public class AuthService : IAuthService
{
    private readonly IRepositoryBase<User> _userRepository;
    private readonly ILogger<AuthService> _logger;
    private readonly JwtTokenLibrary _jwtTokenLibrary;
    private readonly IDistributedCache _cache;
    private readonly AesSecurityLibrary _aesSecurityLibrary;

    public AuthService(IRepositoryBase<User> userRepository,
        ILogger<AuthService> logger,
        JwtTokenLibrary jwtTokenLibrary,
        IDistributedCache cache,
        AesSecurityLibrary aesSecurityLibrary)
    {
        _userRepository = userRepository;
        _logger = logger;
        _jwtTokenLibrary = jwtTokenLibrary;
        _cache = cache;
        _aesSecurityLibrary = aesSecurityLibrary;
    }

    public async Task<LoginResult> TryLoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetAsync(x => x.Account == request.Account);
        if (user is null)
        {
            throw new CommonException("User not found.", StatusCodes.Status401Unauthorized);
        }

        var validationResult = TryLogin(user, request.Password, request.Otp);
        if (validationResult != LoginResultCode.Success)
        {
            throw new CommonException(validationResult.ToString(), StatusCodes.Status401Unauthorized);
        }

        var loginResult = createLoginResult(user);

        await cacheToken(user.Id, loginResult);

        return loginResult;
    }

    public async Task<User?> GetUserByAccountAsync(string account)
    {
        var user = await _userRepository.GetAsync(x => x.Account == account);
        return user;
    }

    public LoginResultCode TryLogin(User user, string password, string? otp)
    {
        // 基本檢查密碼是否有問題
        if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
        {
            _logger.LogWarning(LogMessages.Services.UserService.PasswordIncorrect, user.Account);
            return LoginResultCode.InvalidAccountOrPassword;
        }

        // 檢查帳號是否有啟用
        if (!user.Enable)
        {
            _logger.LogWarning(LogMessages.Services.UserService.UserIsNotEnable, user.Account);
            return LoginResultCode.NotEnable;
        }

        // 如果用戶有申請 TOTP 驗證碼則需要進行驗證
        if (!string.IsNullOrEmpty(user.Secret))
        {
            if (string.IsNullOrEmpty(otp))
            {
                _logger.LogWarning(LogMessages.Services.UserService.OtpIsEmpty, user.Account);
                return LoginResultCode.InvalidAccountOrPassword;
            }

            // TODO: 實作 OTP 驗證功能
            if (user.Secret != otp)
            {
                _logger.LogWarning(LogMessages.Services.UserService.InvalidOtp, user.Account);
                return LoginResultCode.InvalidOtp;
            }
        }

        return LoginResultCode.Success;
    }

    public async Task<LoginResult> RegisterAsync(RegisterRequest request)
    {
        var user = new User
        {
            Account = request.Account,
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Name = request.Name,
            Email = request.Email,
            Enable = true,
            CreatedAt = DateTime.Now
        };

        _userRepository.Create(user);

        var affectedRecord = await _userRepository.SaveChangesAsync();
        _logger.LogInformation(LogMessages.Services.BaseService.CreateAffectRows, nameof(User), affectedRecord);
        if (affectedRecord == 0)
        {
            _logger.LogError(LogMessages.Services.BaseService.AffectedNoRows, nameof(User));
            throw new CommonException("更新異常，請稍後再試。");
        }

        var result = createLoginResult(user);

        await cacheToken(user.Id, result);

        return result;
    }

    #region Private Method

    private LoginResult createLoginResult(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Account),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new("IssuedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
        };

        var accessToken = _jwtTokenLibrary.GenerateJwtToken(claims);
        var csrfToken = Guid.NewGuid()
            .ToString();
        var refreshToken = Guid.NewGuid()
            .ToString();
        var jwtOptions = _jwtTokenLibrary.GetJwtOptions();

        var loginResult = new LoginResult
        {
            AccessToken = accessToken,
            CsrfToken = csrfToken,
            RefreshToken = refreshToken,
            RefreshTokenLifetime = jwtOptions.RefreshTokenLifetime,
            ExpiresIn = jwtOptions.TokenLifetime,
            ExpiredAt = DateTime.Now.AddMinutes(jwtOptions.TokenLifetime),
            User = new()
            {
                Name = user.Name,
                Avatar = "",
                Email = user.Email
            }
        };

        return loginResult;
    }

    private async Task cacheToken(int userId, LoginResult loginInfo)
    {
        await _cache.SetStringAsync($"RefreshToken:{userId}",
            _aesSecurityLibrary.Encrypt(loginInfo.RefreshToken),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(loginInfo.RefreshTokenLifetime)
            });

        await _cache.SetStringAsync($"CsrfToken:{userId}",
            _aesSecurityLibrary.Encrypt(loginInfo.CsrfToken),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(loginInfo.ExpiresIn)
            });
    }

    #endregion
}
