using SafeChat.Domain.Users;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SafeChat.Domain.Keys;

public class EncryptionKey
{
    public Guid Id { get; set; }
    public EncryptionMode Mode { get; set; }
    [NotMapped]
    public string ModeText => Mode.ToString();
    public string Key { get; set; } = string.Empty;
    public string UserId { get; set; }
    
    [JsonIgnore]
    public User User { get; set; }
}
