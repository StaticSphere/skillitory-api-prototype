using FastEndpoints;
using FluentValidation;

namespace Skillitory.Api.Features.Auth.RefreshTokens;

public class RefreshTokensCommandValidator : Validator<RefreshTokensCommand>
{
    public RefreshTokensCommandValidator()
    {
        RuleFor(x => x.AccessToken)
            .NotEmpty();

        RuleFor(x => x.RefreshToken)
            .NotEmpty();
    }
}
