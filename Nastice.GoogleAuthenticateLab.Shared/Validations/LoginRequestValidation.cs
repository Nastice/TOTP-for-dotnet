using FluentValidation;
using Nastice.GoogleAuthenticateLab.Shared.Extensions;
using Nastice.GoogleAuthenticateLab.Shared.Models.Requests;

namespace Nastice.GoogleAuthenticateLab.Shared.Validations;

public class LoginRequestValidation : AbstractValidator<LoginRequest>
{
    public LoginRequestValidation()
    {
        RuleFor(x => x.Account).NotEmpty().WithDisplayName(x => x.Account);
        RuleFor(x => x.Password).NotEmpty().WithDisplayName(x => x.Password);
    }
}
