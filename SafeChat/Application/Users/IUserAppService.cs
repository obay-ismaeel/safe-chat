using SafeChat.Domain.Users;

namespace SafeChat.Application.Users;

public interface IUserAppService
{
    Task<(string Message, bool IsSuccess, User? User)> LoginUserUsingUserNameAsync(string userName, string password);
}
