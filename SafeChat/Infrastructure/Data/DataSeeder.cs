using Microsoft.AspNetCore.Identity;
using SafeChat.Domain.Users;

namespace SafeChat.Infrastructure.Data;

public static class DataSeeder
{
    public static async Task SeedUsersAsync(IServiceScope serviceScope)
    {
        var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<User>>();

        var user1 = new User
        {
            UserName = "User1",
        };
        var user2 = new User
        {
            UserName = "User2",
        };
        var user3 = new User
        {
            UserName = "User3",
        };
        var stringUser = new User
        {
            UserName = "string"
        };


        var password = "password";

        await Task.WhenAll(
            userManager.CreateAsync(user1, password),
            userManager.CreateAsync(user2, password),
            userManager.CreateAsync(user3, password),
            userManager.CreateAsync(stringUser, "string")
        );

    }
}
