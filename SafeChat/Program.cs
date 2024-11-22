using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SafeChat;
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
