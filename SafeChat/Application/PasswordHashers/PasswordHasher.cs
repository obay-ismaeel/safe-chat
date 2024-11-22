using Microsoft.AspNetCore.Identity;
using SafeChat.Domain.Users;

namespace SafeChat.Application.PasswordHashers;

public class PasswordHasher : IPasswordHasher<User>
{
    const char Delimiter = ';';
    public string HashPassword(User user, string password)
    {
        var hasedPassword = PasswordHashingSalting.HashPasword(password, out byte[] salt);

        return string.Join(Delimiter, Convert.ToHexString(salt), hasedPassword);
    }

    public PasswordVerificationResult VerifyHashedPassword(User user, string hashedPassword, string providedPassword)
    {
        var elements = hashedPassword.Split(Delimiter);
        var salt = Convert.FromHexString(elements[0]);
        var hash = elements[1];

        return PasswordHashingSalting.VerifyPassword(providedPassword, hash, salt)
            ? PasswordVerificationResult.Success
            : PasswordVerificationResult.Failed;
    }
}
