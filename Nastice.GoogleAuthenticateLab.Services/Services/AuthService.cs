using Microsoft.Extensions.Logging;
using Nastice.GoogleAuthenticateLab.Data.Interfaces;
using Nastice.GoogleAuthenticateLab.Data.Nastice.GoogleAuthenticateLab.Data.DTOs;
using Nastice.GoogleAuthenticateLab.Services.Interfaces;
using Nastice.GoogleAuthenticateLab.Shared.Enums;
using Nastice.GoogleAuthenticateLab.Shared.Models.Requests;

namespace Nastice.GoogleAuthenticateLab.Services.Services
{
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

        public LoginResultCode LoginAsync(User user, string password)
        {
            return LoginResultCode.Success;
        }

        public async Task<User?> RegisterAsync(RegisterRequest request)
        {
            var user = new User
            {
                Account = request.Account!,
                Password = request.Password!,
                Name = request.Name,
                Email = request.Email,
                Enable = true,
                CreatedAt = DateTime.Now
            };

            _userRepository.Create(user);

            var affectRecord = await _userRepository.SaveChangesAsync();
            if (affectRecord == 0)
            {
                _logger.LogError("");
            }

            return user;
        }
    }
}
