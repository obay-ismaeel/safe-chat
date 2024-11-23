namespace Client.Models;

public static class ClientConstant
{
    public const string BaseUrl = "https://localhost:7239/";
    public const string BaseUrlApi = "https://localhost:7239/api/";
    public const string LoginApi = $"{BaseUrlApi}Auth/login";
    public const string UserApi = $"{BaseUrlApi}Chat/users";
    public const string SignalRChat = "https://localhost:7239/chat-hub";

    public static Dictionary<string, string> AESKeys = [];
    public static Dictionary<string, (string PublicKey, string PrivateKey)> AsymmetricKeys = [];
}
