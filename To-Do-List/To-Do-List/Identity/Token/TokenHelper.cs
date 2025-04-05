using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SelfLearnProject.Entities;
using To_Do_List.Identity.Interface;
using To_Do_List.Identity.Options;

namespace To_Do_List.Identity.Token;

public class TokenHelper(IOptionsSnapshot<JwtTokenOption> jwtTokenOption) : ITokenHelper
{
    public JwtToken CreateToken<T>(T entity) where T : class
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
        return CreateTokenString(claims);
    }

    public JwtToken CreateToken(Dictionary<string, string> KeyValuePairs)
    {
        List<Claim> claims = new List<Claim>();
        foreach (var item in KeyValuePairs)
        {
            claims.Add(new Claim(item.Key, item.Value));
        }

        return CreateTokenString(claims);
    }
    
    private JwtToken CreateTokenString(IEnumerable<Claim> claims)
    {
        DateTime expires = DateTime.Now.AddHours(jwtTokenOption.Value.AccessTokenExpiresMinutes);
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