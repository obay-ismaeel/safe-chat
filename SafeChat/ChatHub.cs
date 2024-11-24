using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SafeChat.Domain.Keys;
using SafeChat.Domain.Messages;
using SafeChat.Infrastructure;
using Shared.Encryptions;
using System.Security.Claims;

namespace SafeChat;

[Authorize]
public class ChatHub : Hub<IChatClient>
{
    private readonly static Dictionary<string, string> _connectedUsers = [];
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
            _connectedUsers.Remove(user.Key);

        return base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(string message, string recieverId, EncryptionMode encryptionMode)
    {
        var result = await UserExists(recieverId);
        if (!result)
            return;

        var senderId = Context.User!.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value;

        await SaveMessageToDb(message, senderId, recieverId, encryptionMode);

        if (_connectedUsers.TryGetValue(recieverId, out var receiverConnectionId))
            await Clients.Client(receiverConnectionId).RecieveMessage(message, encryptionMode, senderId);
    
    }

    public void RegisterUser(string userId)
    {
        _connectedUsers[userId] = Context.ConnectionId;
    }

    public async Task SendKey(string recieverId, string key, EncryptionMode encryptionMode)
    {
        var result = await UserExists(recieverId);
        if (!result)
            return;

        var senderId = Context.User!.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value;

        if (_connectedUsers.TryGetValue(recieverId, out var receiverConnectionId))
            await Clients.Client(receiverConnectionId).RecieveKey(key, encryptionMode, senderId);
    }

    private async Task SaveMessageToDb(string content, string senderId, string recieverId, EncryptionMode encryptionMode)
    {
        var dbMessage = new Message { Content = content, ReceiverId = new Guid(recieverId), SenderId = new Guid(senderId), EncryptionMode = encryptionMode };

        await _context.Messages.AddAsync(dbMessage);

        await _context.SaveChangesAsync();
    }

    private async Task<bool> UserExists(string userId)
    {
        return await _context.Users.FindAsync(userId) is not null;
    }
}
