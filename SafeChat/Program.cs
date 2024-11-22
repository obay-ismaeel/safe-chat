using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SafeChat;
using SafeChat.Application.Encryptions;
using SafeChat.Application.Tokens;
using SafeChat.Application.Users;
using SafeChat.Domain.Users;
using SafeChat.Extensions;
using SafeChat.Infrastructure;
using SafeChat.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<SafeChatDbContext>(
    options => options.UseInMemoryDatabase("SafeChatDb")
);

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 5;

}).AddEntityFrameworkStores<SafeChatDbContext>().AddDefaultTokenProviders();

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddSwaggerDocumentation();

builder.Services.AddScoped<ITokenAppService, TokenAppService>();
builder.Services.AddScoped<IUserAppService, UserAppService>();

builder.Services.AddSignalR();

//// Generate a 256-bit AES key
//string aesKeyBase64 = AESKeyGenerator.GenerateKeyBase64(256);
//Console.WriteLine($"Generated AES Key (Base64): {aesKeyBase64}");

//var encryptService = new EncryptionAES(aesKeyBase64);

//var encryptedMessage = await encryptService.EncryptAsync("Hello, User2!");
//Console.WriteLine($"Encrypted: {encryptedMessage}");

//var decryptedMessage = await encryptService.DecryptAsync(encryptedMessage);
//Console.WriteLine($"Decrypted: {decryptedMessage}");

//var (publicKey, privateKey) = RSAKeyGenerator.GenerateKeys();
//Console.WriteLine($"Public Key: {publicKey}");
//Console.WriteLine($"Private Key: {privateKey}");

//var senderMessage = "Hello, User2!";
//encryptedMessage = AsymmetricEncryptionService.Encrypt(senderMessage, publicKey);
//Console.WriteLine($"Encrypted Message: {encryptedMessage}");

//decryptedMessage = AsymmetricEncryptionService.Decrypt(encryptedMessage, privateKey);
//Console.WriteLine($"Decrypted Message: {decryptedMessage}");


var app = builder.Build();

await DataSeeder.SeedUsersAsync(app.Services.CreateScope());

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHub<ChatHub>("chat-hub");

app.Run();
