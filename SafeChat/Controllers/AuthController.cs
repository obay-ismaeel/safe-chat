using Microsoft.AspNetCore.Mvc;
using SafeChat.Application.Tokens;
using SafeChat.Application.Users;
using SafeChat.Application.Users.DTOs;

namespace SafeChat.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IUserAppService _userAppService;
    private readonly ITokenAppService _tokenAppService;

    public AuthController(IUserAppService userAppService, ITokenAppService tokenAppService) : base()
    {
        _userAppService = userAppService;
        _tokenAppService = tokenAppService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginUserName([FromBody] IncomingUserLoginUsingUserNameDTO incomingUserLoginUsingUserNameDTO)
    {
        var result = await _userAppService.LoginUserUsingUserNameAsync(
            incomingUserLoginUsingUserNameDTO.UserName,
            incomingUserLoginUsingUserNameDTO.Password
        );
        if (!result.IsSuccess)
            return BadRequest(new OutgoingUserLoginDTO
            {
                Message = result.Message,
                IsSuccess = false,
            });

        var (JwtToken, ExpireDate) = await _tokenAppService.GenerateJwtTokenAsync(result.User!);


        return Ok(new OutgoingUserLoginDTO
        {
            Message = result.Message,
            IsSuccess = true,
            JwtToken = JwtToken,
            ExpireDate = ExpireDate
        });
    }   
}
