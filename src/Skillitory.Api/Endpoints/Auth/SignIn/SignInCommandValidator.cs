using FastEndpoints;
using FluentValidation;

namespace Skillitory.Api.Endpoints.Auth.SignIn;

public class SignInCommandValidator : Validator<SignInCommand>
{
    public SignInCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}
