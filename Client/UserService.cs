using Client.Models;
using System.Text.Json;

namespace Client;

public class UserService
{
    private readonly HttpClient _httpClient;
    public UserService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<UserModel>> GetUsersAsync(string jwtToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, ClientConstant.UserApi);
        request.Headers.Add("Authorization", $"Bearer {jwtToken}");

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();

        var users = JsonSerializer.Deserialize<List<UserModel>>(responseContent);

        if (users is null)
            return [];

        Console.WriteLine("\nUsers:\n");
        foreach (var user in users)
            Console.WriteLine($"User name: {user.UserName}");
            //Console.WriteLine($"ID: {user.Id}, Username: {user.UserName}");

        return users;
    }
}
