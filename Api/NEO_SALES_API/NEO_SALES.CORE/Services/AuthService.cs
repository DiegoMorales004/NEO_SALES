using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NEO_SALES.CORE.Interfaces.Services;
using NEO_SALES.CORE.Models.Configuration;
using NEO_SALES.CORE.Models.Dtos.Auth;

namespace NEO_SALES.CORE.Services;

public class AuthService : IAuthService
{
    private readonly AuthCredentialConfiguration _credential;
    private readonly AuthConfig _authConfig;

    public AuthService(IOptions<AuthCredentialConfiguration> credential, IOptions<AuthConfig> authConfig)
    {
        _credential = credential.Value;
        _authConfig = authConfig.Value;
    }

    public LoginResponseDto? Login(LoginRequestDto request)
    {
        if (request.User != _credential.User || request.Password != _credential.Password)
        {
            return null;
        }

        var expiresAtUtc = DateTime.UtcNow.AddMinutes(_authConfig.ExpirationMinutes);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, request.User)
        };

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authConfig.SecretKey));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _authConfig.Issuer,
            claims: claims,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        return new LoginResponseDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAtUtc = expiresAtUtc
        };
    }
}
