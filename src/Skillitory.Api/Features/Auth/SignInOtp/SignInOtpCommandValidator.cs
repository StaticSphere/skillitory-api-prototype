using System.Text.RegularExpressions;
using FastEndpoints;
using FluentValidation;

namespace Skillitory.Api.Features.Auth.SignInOtp;

public partial class SignInOtpCommandValidator : Validator<SignInOtpCommand>
{
    [GeneratedRegex(@"^\d{6}$", RegexOptions.Compiled)]
    private static partial Regex OtpPatternMatcher();

    private static readonly Regex OtpPattern = OtpPatternMatcher();

    public SignInOtpCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Otp)
            .NotEmpty()
            .Matches(OtpPattern);
    }
}
