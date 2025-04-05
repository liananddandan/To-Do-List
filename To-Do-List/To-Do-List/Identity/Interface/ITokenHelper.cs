using SelfLearnProject.Entities;

namespace To_Do_List.Identity.Interface;

public interface ITokenHelper
{
    JwtToken CreateToken<T>(T entity) where T : class;
    JwtToken CreateToken(Dictionary<string, string> claims);
}