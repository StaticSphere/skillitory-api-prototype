using System.Diagnostics.CodeAnalysis;
using FastEndpoints;
using FluentValidation;

namespace Skillitory.Api.Features.Auth.SignIn;

[ExcludeFromCodeCoverage]
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
