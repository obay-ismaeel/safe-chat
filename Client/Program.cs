using Client;
using Client.Models;
using Shared.Encryptions;

// Generate a 256-bit AES key
//string aesKeyBase64 = AESKeyGenerator.GenerateKeyBase64(256);
//var encryptAES = new EncryptionAES(aesKeyBase64);
//var (publicKey, privateKey) = RSAKeyGenerator.GenerateKeys();
//encryptedMessage = AsymmetricEncryptionService.Encrypt(senderMessage, publicKey);
//decryptedMessage = AsymmetricEncryptionService.Decrypt(encryptedMessage, privateKey);


var httpClient = new HttpClient();
var loginService = new LoginService(httpClient);
var userService = new UserService(httpClient);


var loginResponse = new LoginResponse();
while (!loginResponse.IsSuccess)
    loginResponse = await loginService.LoginAsync();

var users = await userService.GetUsersAsync(loginResponse.JwtToken!);

var client = new ChatClient(ClientConstant.SignalRChat, loginResponse.JwtToken!, users);

await client.StartAsync();

Console.WriteLine("Enter User name and message (or type 'q' to quit):\n");
while (true)
{
    await Task.Delay(500);
    
    var userName = GetUserName();
    if (userName == "q") break;

    var message = GetMessage();
    if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(message))
        continue;

    if (!users.Any(x => x.UserName == userName))
    {
        Console.WriteLine("User not found. Please try again.");
        continue;
    }

    var encryptionMode = ChooseEncryptionMode();
    await GenerateKeyBasedOnModeAsync(userName, encryptionMode, client);

    await SendEncryptedMessageAsync(message, userName, encryptionMode, client);
    //await client.SendMessageAsync(message, userName);
}

await client.StopAsync();




#region Methods

async Task SendEncryptedMessageAsync(string message, string userName, EncryptionMode encryptionMode, ChatClient chatClient)
{
    string encryptedMessage = message;

    switch (encryptionMode)
    {
        case EncryptionMode.Symmetric:
            if (!ClientConstant.AESKeys.TryGetValue(userName, out var aesKey))
            {
                Console.WriteLine($"No symmetric key found for {userName}. Generate the key first.");
                return;
            }

            var encryptionAES = new EncryptionAES(aesKey);

            encryptedMessage = await encryptionAES.EncryptAsync(message);
            break;

        case EncryptionMode.Asymmetric:
            if (!ClientConstant.AsymmetricKeys.TryGetValue(userName, out var keys))
            {
                Console.WriteLine($"No asymmetric key found for {userName}. Generate the key first.");
                return;
            }

            var (publicKey, _) = keys;
            encryptedMessage = AsymmetricEncryptionService.Encrypt(message, publicKey);
            break;

        default:
            Console.WriteLine("No encryption mode selected.");
            break;
    }

    await chatClient.SendMessageAsync(encryptedMessage, userName, encryptionMode);
}

EncryptionMode ChooseEncryptionMode()
{
    Console.WriteLine("Available Encryption Modes:");
    foreach (var mode in Enum.GetValues(typeof(EncryptionMode)))
        Console.WriteLine($"- {mode}");

    Console.Write("Choose Mode: ");
    var chosenMode = Console.ReadLine();

    return chosenMode switch
    {
        "Symmetric" => EncryptionMode.Symmetric,
        "Asymmetric" => EncryptionMode.Asymmetric,
        _ => EncryptionMode.None,
    };
}

async Task GenerateKeyBasedOnModeAsync(string userName, EncryptionMode encryptionMode, ChatClient chatClient)
{
    switch (encryptionMode)
    {
        case EncryptionMode.Symmetric:
            if (ClientConstant.AESKeys.ContainsKey(userName))
                break;

            var key = AESKeyGenerator.GenerateKeyBase64(256);

            ClientConstant.AESKeys.Add(userName, key);
            Console.WriteLine("Symmetric key generated.");

            try
            {
                await chatClient.SendKeyAsync(key, encryptionMode, userName);
            }
            catch (Exception e)
            {
                ClientConstant.AsymmetricKeys.Remove(userName);
                Console.WriteLine(e);
                return;
            }

            break;

        case EncryptionMode.Asymmetric:
            if (ClientConstant.AsymmetricKeys.ContainsKey(userName))
                break;

            var (publicKey, privateKey) = RSAKeyGenerator.GenerateKeys();

            ClientConstant.AsymmetricKeys.Add(userName, (string.Empty, privateKey));
            try
            {
                await chatClient.SendKeyAsync(publicKey, encryptionMode, userName);

                await Task.Delay(1000);
            }
            catch (Exception e)
            {
                ClientConstant.AsymmetricKeys.Remove(userName);
                Console.WriteLine(e);
                return;
            }
            
            //ClientConstant.AsymmetricKeys.Add(userName, RSAKeyGenerator.GenerateKeys());
            Console.WriteLine("Asymmetric keys generated.");
            
            break;

        default:
            Console.WriteLine("No encryption selected.");
            break;
    }
}

string? GetUserName()
{
    Console.Write("User Name: ");
    return Console.ReadLine();
}

string? GetMessage()
{
    Console.Write("Message: ");
    return Console.ReadLine();
}

#endregion