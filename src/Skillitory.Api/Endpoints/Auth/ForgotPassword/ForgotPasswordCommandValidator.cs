using FastEndpoints;
using FluentValidation;

namespace Skillitory.Api.Endpoints.Auth.ForgotPassword;

public class ForgotPasswordCommandValidator : Validator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}
