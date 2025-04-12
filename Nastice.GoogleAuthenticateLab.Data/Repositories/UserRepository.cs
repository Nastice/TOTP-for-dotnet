using Microsoft.EntityFrameworkCore;
using Nastice.GoogleAuthenticateLab.Data.Nastice.GoogleAuthenticateLab.Data.DTOs;

namespace Nastice.GoogleAuthenticateLab.Data.Repositories;

public class UserRepository : BaseRepository<User>
{
    public UserRepository(DbContext context) : base(context)
    {
    }
}
