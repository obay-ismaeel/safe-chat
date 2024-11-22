using Client;
using Client.Models;

var httpClient = new HttpClient();
var loginService = new LoginService(httpClient);
var userService = new UserService(httpClient);

var loginResponse = new LoginResponse();
while (!loginResponse.IsSuccess)
{
    loginResponse = await loginService.LoginAsync();
    
    if (loginResponse.IsSuccess)
        break;
}

var users = await userService.GetUsersAsync(loginResponse.JwtToken!);

var client = new ChatClient(Constant.SignalRChat, loginResponse.JwtToken!, users);

await client.StartAsync();

Console.WriteLine("Enter User name and message (or type 'q' to quit):\n");
while (true)
{
    await Task.Delay(500);

    Console.Write("User Name: ");
    var userName = Console.ReadLine();

    if (userName == "'q'") break;

    Console.Write("Message: ");
    var message = Console.ReadLine();

    if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(message))
        continue;

    await client.SendMessageAsync(message, userName);
}

await client.StopAsync();


