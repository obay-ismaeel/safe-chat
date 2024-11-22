using Microsoft.AspNetCore.SignalR;
using System.Runtime.InteropServices;

namespace SafeChat;

using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class ChatHub : Hub
{
    private static Dictionary<string, string> _connectedUsers = new Dictionary<string, string>();

    // Method to map user to a connection
    public override Task OnConnectedAsync()
    {
        // Optionally handle user connections
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        // Remove user from connected users
        var user = _connectedUsers.FirstOrDefault(x => x.Value == Context.ConnectionId);
        if (user.Key != null)
        {
            _connectedUsers.Remove(user.Key);
        }
        return base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(string receiver, string message)
    {
        if (_connectedUsers.TryGetValue(receiver, out var receiverConnectionId))
        {
            // Send the message to the specific client
            await Clients.Client(receiverConnectionId).SendAsync("ReceiveMessage", message);
        }
        else
        {
            await Clients.Caller.SendAsync("Error", "User not connected.");
        }
    }

    public void RegisterUser(string userId)
    {
        _connectedUsers[userId] = Context.ConnectionId;
    }
}
