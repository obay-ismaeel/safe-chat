using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Client.Models;

public class LoginRequest
{
    [JsonPropertyName("userName")]
    public string UserName { get; set; } = string.Empty;
    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;
}
