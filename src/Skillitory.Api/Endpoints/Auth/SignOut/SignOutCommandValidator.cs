using FastEndpoints;
using FluentValidation;

namespace Skillitory.Api.Endpoints.Auth.SignOut;

public class SignOutCommandValidator : Validator<SignOutCommand>
{
    public SignOutCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .When(x => !x.IsBrowser);
    }
}
