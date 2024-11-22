using System.Security.Cryptography;

namespace SafeChat.Application.Encryptions;

public class RSAKeyGenerator
{
    public static (string publicKey, string privateKey) GenerateKeys()
    {
        using var rsa = RSA.Create();

        var publicKey = Convert.ToBase64String(rsa.ExportRSAPublicKey());

        var privateKey = Convert.ToBase64String(rsa.ExportRSAPrivateKey());

        return (publicKey, privateKey);
    }
}

