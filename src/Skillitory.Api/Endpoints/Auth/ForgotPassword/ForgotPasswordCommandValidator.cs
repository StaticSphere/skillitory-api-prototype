using System.Diagnostics.CodeAnalysis;
using FastEndpoints;
using FluentValidation;

namespace Skillitory.Api.Endpoints.Auth.ForgotPassword;

[ExcludeFromCodeCoverage]
public class ForgotPasswordCommandValidator : Validator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}
