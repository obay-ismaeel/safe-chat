namespace Client;

using System;
using System.Threading.Tasks;
using Client.Models;
using Microsoft.AspNetCore.SignalR.Client;

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

        _connection.On<string, string>("RecieveMessage", (message, senderId) =>
        {
            var sender = _users.FirstOrDefault(x => x.Id == new Guid(senderId));
            if(sender is null)
            {
                Console.WriteLine($"\nMessage from {senderId}: {message} \n");
                return;
            }

            Console.WriteLine($"\nMessage from {sender.UserName}: {message} \n");
        });
    }

    public async Task StartAsync()
    {
        await _connection.StartAsync();
        Console.WriteLine("\nConnected to the chat hub\n");
    }

    public async Task SendMessageAsync(string message, string userName)
    {
        var recieverId = _users.FirstOrDefault(x => x.UserName == userName)?.Id;
        if (recieverId is null)
            return;

        await _connection.InvokeAsync("SendMessage", message, recieverId);
    }

    public async Task StopAsync()
    {
        await _connection.StopAsync();
        Console.WriteLine("\nDisconnected from the chat hub\n");
    }
}
