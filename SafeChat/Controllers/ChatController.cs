using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SafeChat.Infrastructure;

namespace SafeChat.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly SafeChatDbContext _context;

    public ChatController(SafeChatDbContext context)
    {
        _context = context;
    }

    [HttpGet("messages")]
    public async Task<IActionResult> GetAllMessages()
    {
        return Ok(await _context.Messages.ToListAsync());
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
        return Ok(await _context.Users.ToListAsync());
    }
}
