using FastEndpoints;
using FluentValidation;

namespace Skillitory.Api.Features.Auth.SignOutAllDevices;

public class SignOutAllDevicesCommandValidator : Validator<SignOutAllDevicesCommand>
{
    public SignOutAllDevicesCommandValidator()
    {
        RuleFor(x => x.AccessToken)
            .NotEmpty();
    }
}
