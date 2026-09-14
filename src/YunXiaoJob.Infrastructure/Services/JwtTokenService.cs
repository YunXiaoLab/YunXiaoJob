using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using YunXiaoJob.Application.Interfaces.Services;
using YunXiaoJob.Domain.Enums;
using YunXiaoJob.Infrastructure.Settings;
namespace YunXiaoJob.Infrastructure.Services;
public class JwtTokenService : IAuthTokenService
{
    private readonly JwtSettings _settings;
    public JwtTokenService(JwtSettings settings) { _settings = settings; if (Encoding.UTF8.GetByteCount(settings.SigningKey) < 32) throw new InvalidOperationException("JWT signing key must contain at least 32 bytes."); }
    public string CreateAccessToken(Guid userId, string email, PlatformRole platformRole)
    {
        var claims = new[] { new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()), new Claim(JwtRegisteredClaimNames.Email, email), new Claim(ClaimTypes.Role, platformRole.ToString()) };
        var credentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SigningKey)), SecurityAlgorithms.HmacSha256);
        return new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(_settings.Issuer, _settings.Audience, claims, expires: DateTime.UtcNow.AddMinutes(_settings.AccessTokenMinutes), signingCredentials: credentials));
    }
}
