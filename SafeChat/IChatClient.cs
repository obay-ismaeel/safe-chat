using Shared.Encryptions;

namespace SafeChat;

public interface IChatClient
{
    Task RecieveMessage(string message, EncryptionMode encryptionMode, string senderId);
    Task RecieveKey(string key, EncryptionMode encryptionMode, string senderId);
}
