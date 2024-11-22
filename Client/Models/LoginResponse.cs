namespace Client.Models;

public class LoginResponse
{
    public string Message { get; set; } = string.Empty;
    public bool IsSuccess { get; set; }
    public DateTime? ExpireDate { get; set; }
    public string? JwtToken { get; set; }
}
