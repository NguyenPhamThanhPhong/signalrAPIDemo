using Microsoft.IdentityModel.Tokens;
using signalr.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace signalr.Services
{
    public class TokenGenerator
    {
        private readonly TokenConfigs _tokenConfigs;

        public TokenGenerator(TokenConfigs tokenConfigs)
        {
            _tokenConfigs = tokenConfigs;
        }
        public string GenerateAccessToken(User user)
        {
            SecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _tokenConfigs.AccessTokenSecret));
            SigningCredentials credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id),
                new Claim(ClaimTypes.Name,user.Name),
            };

            JwtSecurityToken token = new JwtSecurityToken(
                _tokenConfigs.Issuer,
                _tokenConfigs.Audience,
                claims,
                DateTime.UtcNow,
                DateTime.UtcNow.AddMinutes(_tokenConfigs.AccessTokenExpirationMinutes),
                credential
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
