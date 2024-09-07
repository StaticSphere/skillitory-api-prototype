using FastEndpoints;
using FluentValidation;

namespace Skillitory.Api.Endpoints.Auth.ResetPassword;

public class ResetPasswordCommandValidator : Validator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.Token).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}
