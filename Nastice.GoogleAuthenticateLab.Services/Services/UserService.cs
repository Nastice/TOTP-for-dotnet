using System.Security.Claims;
using Nastice.GoogleAuthenticateLab.Data.Interfaces;
using Nastice.GoogleAuthenticateLab.Data.Nastice.GoogleAuthenticateLab.Data.DTOs;
using Nastice.GoogleAuthenticateLab.Data.Repositories;
using Nastice.GoogleAuthenticateLab.Services.Interfaces;

namespace Nastice.GoogleAuthenticateLab.Services.Services;

public class UserService : IUserService
{
    private readonly IRepositoryBase<User> _userRepository;
    public UserService(IRepositoryBase<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User?> GetUserByClaims(ClaimsPrincipal userClaims)
    {
        var userId = userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return null;
        }

        var user = await _userRepository.GetAsync(x => x.Id == int.Parse(userId));
        return user;
    }
}
