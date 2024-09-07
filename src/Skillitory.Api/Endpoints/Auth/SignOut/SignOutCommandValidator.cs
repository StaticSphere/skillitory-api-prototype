using FastEndpoints;
using FluentValidation;

namespace Skillitory.Api.Endpoints.Auth.SignOut;

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
