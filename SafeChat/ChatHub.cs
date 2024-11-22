using Microsoft.AspNetCore.SignalR;
using System.Runtime.InteropServices;

namespace SafeChat;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SafeChat.Infrastructure;
using System.ComponentModel;
using System.Security.Claims;
using System.Threading.Tasks;

[Authorize]
public class ChatHub : Hub<IChatClient>
{
    private static Dictionary<string, string> _connectedUsers = new Dictionary<string, string>();
    private readonly SafeChatDbContext _context;

    public ChatHub(SafeChatDbContext context)
    {
        _context = context;
    }

    public override Task OnConnectedAsync()
    {
        var userId = Context.User!.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value;
        _connectedUsers.Add(userId, Context.ConnectionId);
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var user = _connectedUsers.FirstOrDefault(x => x.Value == Context.ConnectionId);
        if (user.Key != null)
        {
            _connectedUsers.Remove(user.Key);
        }
        return base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(string message, string recieverId)
    {
        var result = await UserExists(recieverId);
        if (!result)
            return;

        var senderId = Context.User!.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value;

        await SaveMessageToDb(message, senderId, recieverId);

        if (_connectedUsers.TryGetValue(recieverId, out var receiverConnectionId))
        {
            await Clients.Client(receiverConnectionId).RecieveMessage(message, senderId);
        }
    }

    public void RegisterUser(string userId)
    {
        _connectedUsers[userId] = Context.ConnectionId;
    }

    private async Task SaveMessageToDb(string content, string senderId, string recieverId)
    {
        var dbMessage = new Message { Content = content, ReceiverId = new Guid(recieverId), SenderId = new Guid(senderId) };

        await _context.Messages.AddAsync(dbMessage);

        await _context.SaveChangesAsync();
    }

    private async Task<bool> UserExists(string userId)
    {
        return await _context.Users.FindAsync(userId) is not null;
    }
}
