namespace SafeChat;

public interface IChatClient
{
    Task RecieveMessage(string message, string senderId);
}
