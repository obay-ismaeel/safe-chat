﻿namespace SafeChat.Application.Users.DTOs;

public class OutgoingUserLoginDTO
{
    public string Message { get; set; } = string.Empty;
    public bool IsSuccess { get; set; }
    public DateTime? ExpireDate { get; set; }
    public string? JwtToken { get; set; }
}
