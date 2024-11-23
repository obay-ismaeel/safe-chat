using Client;
using Client.Encryptions;
using Client.Models;

var httpClient = new HttpClient();
var loginService = new LoginService(httpClient);
var userService = new UserService(httpClient);

var loginResponse = new LoginResponse();
while (!loginResponse.IsSuccess)
    loginResponse = await loginService.LoginAsync();

var users = await userService.GetUsersAsync(loginResponse.JwtToken!);

var client = new ChatClient(Constant.SignalRChat, loginResponse.JwtToken!, users);

await client.StartAsync();

Console.WriteLine("Enter User name and message (or type 'q' to quit):\n");
while (true)
{
    await Task.Delay(500);

    Console.WriteLine("Available Encryption Modes:");
    foreach (var mode in Enum.GetValues(typeof(EncryptionMode)))
        Console.WriteLine($"- {mode}");

    Console.WriteLine("Choose Mode: ");
    var choosenMode = Console.ReadLine();

    var encryptionMode = choosenMode switch
    {
        null => EncryptionMode.None,
        "Symmetric" => EncryptionMode.Symmetric,
        "Asymmmetric" => EncryptionMode.Asymmmetric,
        _ => EncryptionMode.None,
    };

    Console.Write("User Name: ");
    var userName = Console.ReadLine();

    if (userName is "q") break;

    Console.Write("Message: ");
    var message = Console.ReadLine();

    if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(message))
        continue;

    if (!users.Any(x => x.UserName == userName))
        continue;

    await client.SendMessageAsync(message, userName);
}

await client.StopAsync();