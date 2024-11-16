namespace Skillitory.Api.Services.Interfaces;

public interface ICookieService
{
    void SetRefreshTokenCookie(string refreshToken, DateTimeOffset refreshTokenExpiration);
    void ClearRefreshTokenCookie();
}
