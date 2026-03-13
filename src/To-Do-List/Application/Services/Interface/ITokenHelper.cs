using System.Security.Claims;
using To_Do_List.Application.Common.Token;
using To_Do_List.Application.DTOs;

namespace To_Do_List.Application.Services.Interface;

public interface ITokenHelper
{
    JwtToken CreateToken<T>(T entity, TokenType type) where T : class;
    JwtToken CreateToken(Dictionary<string, string> claims, TokenType type);
    (ApiResponseCode code, ClaimsPrincipal? principal) ValidateExpiredRefreshToken(string refreshToken);
}