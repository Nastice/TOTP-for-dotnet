using Microsoft.Extensions.Logging;
using Nastice.GoogleAuthenticateLab.Data.Interfaces;
using Nastice.GoogleAuthenticateLab.Data.Nastice.GoogleAuthenticateLab.Data.DTOs;
using Nastice.GoogleAuthenticateLab.Services.Interfaces;
using Nastice.GoogleAuthenticateLab.Shared.Enums;
using Nastice.GoogleAuthenticateLab.Shared.Exceptions;
using Nastice.GoogleAuthenticateLab.Shared.Models.Requests;
using Nastice.GoogleAuthenticateLab.Shared.Models.Responses;
using Nastice.GoogleAuthenticateLab.Shared.Resources;

namespace Nastice.GoogleAuthenticateLab.Services.Services;

public class AuthService : IAuthService
{
    private readonly IRepositoryBase<User> _userRepository;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IRepositoryBase<User> userRepository, ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<User?> GetUserByAccountAsync(string account)
    {
        var user = await _userRepository.GetAsync(x => x.Account == account);
        return user;
    }

    public LoginResultCode LoginAsync(User user, string password, string? otp)
    {
        // 基本檢查密碼是否有問題
        if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
        {
            _logger.LogWarning(LogMessages.Services.UserService.PasswordIncorrect, user.Account);
            return LoginResultCode.InvalidAccountOrPassword;
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

        // 檢查帳號是否有啟用
        if (!user.Enable)
        {
            _logger.LogWarning(LogMessages.Services.UserService.UserIsNotEnable, user.Account);
            return LoginResultCode.NotEnable;
        }

        return LoginResultCode.Success;
    }

    public LoginResponse CreateToken(User user)
    {
        return new();
    }

    public async Task<User?> RegisterAsync(RegisterRequest request)
    {
        var user = new User
        {
            Account = request.Account!,
            Password = request.Password!,
            Name = request.Name!,
            Email = request.Email!,
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

        return user;
    }
}