using System.Diagnostics.CodeAnalysis;
using FastEndpoints;
using FluentValidation;

namespace Skillitory.Api.Endpoints.Auth.RefreshTokens;

[ExcludeFromCodeCoverage]
public class RefreshTokensCommandValidator : Validator<RefreshTokensCommand>
{
    public RefreshTokensCommandValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}
