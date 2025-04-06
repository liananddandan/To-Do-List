using To_Do_List.Identity.Token;

namespace To_Do_List.Identity.Interface;

public interface ITokenHelper
{
    JwtToken CreateToken<T>(T entity) where T : class;
    JwtToken CreateToken(Dictionary<string, string> claims);
}