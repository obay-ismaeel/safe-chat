using SafeChat.Domain.Users;

namespace SafeChat.Application.Tokens;

public interface ITokenAppService
{
    Task<(string JwtToken, DateTime ExpireDate)> GenerateJwtTokenAsync(User user);
}
