using System.Diagnostics.CodeAnalysis;
using FastEndpoints;
using FluentValidation;

namespace Skillitory.Api.Endpoints.Auth.RegisterUser;

[ExcludeFromCodeCoverage]
public class RegisterUserCommandValidator : Validator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.LastName).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}
