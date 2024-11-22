namespace SafeChat.Infrastructure;

public class Message
{
    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }
    public string Content { get; set; } = string.Empty;
}
