﻿using SafeChat.Domain.Keys;
using Shared.Encryptions;
using System.ComponentModel.DataAnnotations.Schema;

namespace SafeChat.Domain.Messages;

public class Message
{
    public Guid Id { get; set; }
    public Guid SenderId { get; set; }
    public Guid ReceiverId { get; set; }
    public EncryptionMode EncryptionMode { get; set; }

    [NotMapped]
    public string EncryptionModeText => EncryptionMode.ToString();
    public string Content { get; set; } = string.Empty;
}
