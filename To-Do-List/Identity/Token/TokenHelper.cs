using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using To_Do_List.Identity.Interface;
using To_Do_List.Identity.Options;
using To_Do_List.Require;

namespace To_Do_List.Identity.Token;

public class TokenHelper(IOptionsSnapshot<JwtTokenOption> jwtTokenOption) : ITokenHelper
{
    public JwtToken CreateToken<T>(T entity, TokenType type) where T : class
    {
        List<Claim> claims = new List<Claim>();
        foreach (var item in entity.GetType().GetProperties())
        {
            object obj = item.GetValue(entity);
            string value = "";
            if (obj != null)
            {
                value = obj.ToString();
            }
            claims.Add(new Claim(item.Name, value));
        }

        if (type == TokenType.AccessToken)
        {
            claims.Add(new Claim("token_type", "access_token"));
            return CreateAccessTokenInner(claims);
        }
        else
        {
            claims.Add(new Claim("token_type", "refresh_token"));
            return CreateRefreshTokenInner(claims);
        }
    }

    public JwtToken CreateToken(Dictionary<string, string> keyValuePairs, TokenType type)
    {
        List<Claim> claims = new List<Claim>();
        foreach (var item in keyValuePairs)
        {
            claims.Add(new Claim(item.Key, item.Value));
        }

        if (type == TokenType.AccessToken)
        {
            return CreateAccessTokenInner(claims);
        }
        else
        {
            return CreateRefreshTokenInner(claims);
        }
    }

    private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters()
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidIssuer = jwtTokenOption.Value.Issuer,
            ValidAudience = jwtTokenOption.Value.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtTokenOption.Value.IssuerSigningKey)),
            ValidateLifetime = false // 允许解析过期的token, 为了后续比如查看是哪个用户，以及其他安全措施的实施
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
            if (securityToken is JwtSecurityToken jwtToken &&
                jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return principal;
            }
        }
        catch (Exception e)
        {
            return null;
        }

        return null;
    }

    public (ApiResponseCode code, ClaimsPrincipal? principal) ValidateExpiredRefreshToken(string refreshToken)
    {
        var principal = GetPrincipalFromExpiredToken(refreshToken);
        if (principal == null)
        {
            return (ApiResponseCode.RefreshTokenInvalid, null);
        }

        var jwtTokenType = principal.FindFirst("token_type")?.Value;
        if (jwtTokenType == null || jwtTokenType != "refresh_token")
        {
            return (ApiResponseCode.RefreshTokenTypeWrong, null);
        }
        
        var expiresAt = principal.FindFirst("exp")?.Value;
        if (string.IsNullOrEmpty(expiresAt))
        {
            return (ApiResponseCode.RefreshTokenWithoutExpire, null);
        }
        var expireDateTimeUtc = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expiresAt)).UtcDateTime;
        if (DateTime.UtcNow > expireDateTimeUtc)
        {
            return (ApiResponseCode.RefreshTokenExpired, null);
        }

        var userId = principal.FindFirst("UserId")?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return (ApiResponseCode.RefreshTokenWithoutUserId, null);
        }

        var jwtVersion = principal.FindFirst("JWTVersion")?.Value;
        if (jwtVersion == null)
        {
            return (ApiResponseCode.RefreshTokenWithoutVersion, null);
        }

        return (ApiResponseCode.RefreshTokenInfoCheckSuccess, principal);
    }

    private JwtToken CreateRefreshTokenInner(IEnumerable<Claim> claims)
    {
        DateTime expires = DateTime.Now.AddDays(jwtTokenOption.Value.RefreshTokenExpiresDays);
        return CreateTokenString(claims, expires);
    }

    private JwtToken CreateAccessTokenInner(IEnumerable<Claim> claims)
    {
        DateTime expires = DateTime.Now.AddMinutes(jwtTokenOption.Value.AccessTokenExpiresMinutes);
        return CreateTokenString(claims, expires);
    }

    private JwtToken CreateTokenString(IEnumerable<Claim> claims, DateTime expires)
    {
        var token = new JwtSecurityToken(
            issuer: jwtTokenOption.Value.Issuer,
            audience: jwtTokenOption.Value.Audience,
            claims: claims,
            notBefore: DateTime.Now,
            expires: expires,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtTokenOption.Value.IssuerSigningKey)),
                SecurityAlgorithms.HmacSha256
            )
        );
        return new JwtToken()
        {
            TokenStr = new JwtSecurityTokenHandler().WriteToken(token),
            Expires = expires
        };
    }
}