using FastEndpoints;
using FluentValidation;

namespace Skillitory.Api.Features.Auth.SignOut;

public class SignOutCommandValidator : Validator<SignOutCommand>
{
    public SignOutCommandValidator()
    {
        RuleFor(x => x.AccessToken)
            .NotEmpty();

        RuleFor(x => x.RefreshToken)
            .NotEmpty();
    }
}
