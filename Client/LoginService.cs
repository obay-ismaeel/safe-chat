using Client.Models;
using System.Text.Json;
using System.Text;

namespace Client;

public class LoginService
{
    private readonly HttpClient _httpClient;

    public LoginService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<LoginResponse> LoginAsync()
    {
        //var loginRequest = new LoginRequest
        //{
        //    UserName = "string",
        //    Password = "string"
        //};
        var loginRequest = GetUserNameAndPassword();

        // Serialize the loginRequest object to JSON
        var jsonContent = JsonSerializer.Serialize(loginRequest);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        // Create and send the HTTP POST request
        var response = await _httpClient.PostAsync(Constant.LoginApi, content);

        //// Ensure the response indicates success
        //response.EnsureSuccessStatusCode();

        // Read and deserialize the response content into a LoginResponse object
        var responseContent = await response.Content.ReadAsStringAsync();
        var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true // Ensure correct mapping regardless of case
        });

        return loginResponse ?? new LoginResponse { IsSuccess = false, Message = "Invalid response" };
    }

    private LoginRequest GetUserNameAndPassword()
    {
        Console.WriteLine("Enter User name and password:\n");

        var userName = string.Empty;
        var password = string.Empty;

        while (true)
        {
            Console.Write("User Name: ");
            userName = Console.ReadLine();

            Console.Write("Password: ");
            password = Console.ReadLine();

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                continue;

            break;
        }

        return new LoginRequest { UserName = userName, Password = password };
    }
}
