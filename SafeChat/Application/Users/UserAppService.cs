using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using SafeChat.Domain.Users;

namespace SafeChat.Application.Users;

public class UserAppService : IUserAppService
{
    private readonly UserManager<User> _userManager;

    public UserAppService(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<(string Message, bool IsSuccess, User? User)> LoginUserUsingUserNameAsync(string userName, string password)
    {
        var user = await _userManager.FindByNameAsync(userName);
        if (user is null)
            return (Message: "There is a user with that UserName.", IsSuccess: false, User: null);

        var result = await _userManager.CheckPasswordAsync(user, password);
        if (!result)
            return (Message: "UserName or Password is not correct.", IsSuccess: false, User: null);

        return (Message: "Logged in Successfully!", IsSuccess: true, User: user);
    }
}
