using System.Diagnostics.CodeAnalysis;
using FastEndpoints;
using FluentValidation;

namespace Skillitory.Api.Endpoints.Auth.ValidateUser;

[ExcludeFromCodeCoverage]
public class ValidateUserCommandValidator : Validator<ValidateUserCommand>
{
    public ValidateUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Token)
            .NotEmpty();
    }
}
