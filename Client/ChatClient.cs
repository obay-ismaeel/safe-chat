namespace Client;

using System;
using System.Threading.Tasks;
using Client.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Shared.Encryptions;

public class ChatClient
{
    private readonly HubConnection _connection;
    private readonly List<UserModel> _users;

    public ChatClient(string hubUrl, string jwtToken, List<UserModel> users)
    {
        _users = users;

        _connection = new HubConnectionBuilder()
            .WithUrl(hubUrl, options =>
            {
                options.Headers["Authorization"] = $"Bearer {jwtToken}"; // Add the JWT token to the Authorization header
            })
            .Build();

        _connection.On("RecieveMessage", (Func<string, EncryptionMode, string, Task>)(async (message, encryptionMode, senderId) =>
        {
            var sender = _users.FirstOrDefault(x => x.Id == new Guid(senderId));
            if (sender is null)
                return;

            string decryptedMessage = await DecryptMessageAsync(message, encryptionMode, sender.UserName);

        }));

        _connection.On<string, EncryptionMode, string>("RecieveKey", async (key, encryptionMode, senderId) =>
        {
            var sender = _users.FirstOrDefault(x => x.Id == new Guid(senderId));
            if (sender is null)
                return;

            var senderName = sender.UserName;

            Console.WriteLine($"\nReceived key from {senderName}:");
            Console.WriteLine($"Key: {key}");
            Console.WriteLine($"Encryption Mode: {encryptionMode}\n");

            await SaveKeyBasedOnMode(senderName, key, encryptionMode);
        });

        _connection.Closed += async (exception) =>
        {
            Console.WriteLine("Connection lost. Trying to reconnect...");
            await Task.Delay(500);
            await _connection.StartAsync();
        };

        _connection.Reconnecting += async (exception) =>
        {
            Console.WriteLine("Reconnecting...");
            await Task.Delay(500);
        };
    }

    private async Task<string> DecryptMessageAsync(string message, EncryptionMode encryptionMode, string userName)
    {
        string decryptedMessage = message;

        try
        {
            switch (encryptionMode)
            {
                case EncryptionMode.Symmetric:
                    if (!ClientConstant.AESKeys.TryGetValue(userName, out var aesKey))
                    {
                        Console.WriteLine($"No symmetric key found for {userName}.");
                        return string.Empty;
                    }

                    var aesDecryptor = new EncryptionAES(aesKey);
                    decryptedMessage = await aesDecryptor.DecryptAsync(message);
                    break;

                case EncryptionMode.Asymmetric:
                    if (!ClientConstant.AsymmetricKeys.TryGetValue(userName, out var keys))
                    {
                        Console.WriteLine($"No asymmetric key found for {userName}.");
                        return string.Empty;
                    }

                    var (_, privateKey) = keys;
                    decryptedMessage = AsymmetricEncryptionService.Decrypt(message, privateKey);
                    break;

                default:
                    decryptedMessage = message;
                    break;
            }

            Console.WriteLine($"\nMessage from {userName}: {decryptedMessage} \n");

            return decryptedMessage;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to decrypt message: {ex.Message}");
            return string.Empty;
        }
    }

    public async Task StartAsync()
    {
        await _connection.StartAsync();
        Console.WriteLine("\nConnected to the chat hub\n");
    }

    public async Task SendMessageAsync(string message, string userName, EncryptionMode encryptionMode)
    {
        if (_connection.State != HubConnectionState.Connected)
        {
            Console.WriteLine("Connection is not active. Trying to reconnect...");
            await StartAsync();  // Reconnect if necessary
        }

        var recieverId = _users.FirstOrDefault(x => x.UserName == userName)?.Id;
        if (recieverId is null)
            return;

        await _connection.InvokeAsync("SendMessage", message, recieverId, encryptionMode);
    }

    public async Task SendKeyAsync(string key, EncryptionMode encryptionMode, string userName)
    {
        var receiverId = _users.FirstOrDefault(x => x.UserName == userName)?.Id;
        if (receiverId is null)
            throw new ArgumentNullException(nameof(receiverId));

        await _connection.InvokeAsync("SendKey", receiverId.ToString(), key, encryptionMode);
        Console.WriteLine($"\nKey sent to {userName}\n");
    }

    public async Task StopAsync()
    {
        await _connection.StopAsync();
        Console.WriteLine("\nDisconnected from the chat hub\n");
    }

    private async Task SaveKeyBasedOnMode(string userName, string key, EncryptionMode encryptionMode)
    {
        switch (encryptionMode)
        {
            case EncryptionMode.Symmetric:
                ClientConstant.AESKeys.Add(userName, key);
                break;

            case EncryptionMode.Asymmetric:
                if (!ClientConstant.AsymmetricKeys.ContainsKey(userName))
                {
                    var (publicKey, myPrivateKey) = RSAKeyGenerator.GenerateKeys();

                    ClientConstant.AsymmetricKeys.Add(userName, (key, myPrivateKey));

                    await SendKeyAsync(publicKey, encryptionMode, userName);
                }

                var (_, privateKey) = ClientConstant.AsymmetricKeys[userName];

                ClientConstant.AsymmetricKeys[userName] = (key, privateKey);
                break;

            default:
                break;
        }
    }

}
