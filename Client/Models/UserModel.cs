using System.Text.Json.Serialization;

namespace Client.Models;

public class UserModel
{
    
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
    
    [JsonPropertyName("userName")]
    public string UserName { get; set; } = string.Empty;
    
    [JsonPropertyName("normalizedUserName")]
    public string NormalizedUserName { get; set; } = string.Empty;
    
    [JsonPropertyName("email")]
    public string? Email { get; set; }
    
    [JsonPropertyName("emailConfirmed")]
    public bool EmailConfirmed { get; set; }
    
    [JsonPropertyName("passwordHash")]
    public string PasswordHash { get; set; } = string.Empty;
    
    [JsonPropertyName("securityStamp")]
    public string SecurityStamp { get; set; } = string.Empty;
    
    [JsonPropertyName("concurrencyStamp")]
    public string ConcurrencyStamp { get; set; } = string.Empty;
    
    [JsonPropertyName("phoneNumber")]
    public string? PhoneNumber { get; set; }
    
    [JsonPropertyName("phoneNumberConfirmed")]
    public bool PhoneNumberConfirmed { get; set; }
    
    [JsonPropertyName("twoFactorEnabled")]
    public bool TwoFactorEnabled { get; set; }
    
    [JsonPropertyName("lockoutEnabled")]
    public bool LockoutEnabled { get; set; }
    
    [JsonPropertyName("accessFailedCount")]
    public int AccessFailedCount { get; set; }
}
