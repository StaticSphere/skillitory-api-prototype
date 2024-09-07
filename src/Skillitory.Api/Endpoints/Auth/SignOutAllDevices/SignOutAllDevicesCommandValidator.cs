using FastEndpoints;
using FluentValidation;

namespace Skillitory.Api.Endpoints.Auth.SignOutAllDevices;

public class SignOutAllDevicesCommandValidator : Validator<SignOutAllDevicesCommand>
{
    public SignOutAllDevicesCommandValidator()
    {
        RuleFor(x => x.AccessToken)
            .NotEmpty();
    }
}
