using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SafeChat.Domain.Users;

namespace SafeChat.Infrastructure;

public class SafeChatDbContext : IdentityDbContext<User>
{
    public DbSet<Message> Messages { get; set; }
    public SafeChatDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}
