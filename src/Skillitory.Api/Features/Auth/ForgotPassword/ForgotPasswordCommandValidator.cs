using FastEndpoints;
using FluentValidation;

namespace Skillitory.Api.Features.Auth.ForgotPassword;

public class ForgotPasswordCommandValidator : Validator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}
