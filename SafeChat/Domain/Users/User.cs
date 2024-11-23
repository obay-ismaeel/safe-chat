using Microsoft.AspNetCore.Identity;
using SafeChat.Domain.Keys;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SafeChat.Domain.Users;

public class User : IdentityUser
{
}
