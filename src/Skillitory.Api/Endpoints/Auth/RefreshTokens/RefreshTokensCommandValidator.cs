using FastEndpoints;
using FluentValidation;

namespace Skillitory.Api.Endpoints.Auth.RefreshTokens;

public class RefreshTokensCommandValidator : Validator<RefreshTokensCommand>
{
    public RefreshTokensCommandValidator()
    {
        RuleFor(x => x.AccessToken)
            .NotEmpty()
            .When(x => !x.UseCookies);

        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .When(x => !x.UseCookies);
    }
}
