using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SafeChat.Domain.Users;
using SafeChat.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SafeChat.Application.Tokens;

public class TokenAppService : ITokenAppService
{
    private readonly JwtOptions _options;
    private readonly UserManager<User> _userManager;
    private readonly TokenValidationParameters _tokenValidationParameters;

    public TokenAppService(IOptionsMonitor<JwtOptions> options, UserManager<User> userManager, TokenValidationParameters tokenValidationParameters)
    {
        _options = options.CurrentValue;
        _userManager = userManager;
        _tokenValidationParameters = tokenValidationParameters;
    }


    public Task<(string JwtToken, DateTime ExpireDate)> GenerateJwtTokenAsync(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var signingKey = Encoding.ASCII.GetBytes(_options.SigningKey);

        List<Claim> claims =
        [
            new("Id", user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.GivenName, user.UserName!),
            //new(JwtRegisteredClaimNames.Sub, user.Email!), // unique id
            //new(JwtRegisteredClaimNames.Email, user.Email!),
            //new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // used by refresh token
        ];

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.Add(_options.ExpireTime),
            Audience = _options.ValidAudience,
            Issuer = _options.ValidIssuer,
            SigningCredentials = new SigningCredentials
            (
                new SymmetricSecurityKey(signingKey),
                SecurityAlgorithms.HmacSha256Signature
            ),
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        //var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);

        var jwtToken = tokenHandler.WriteToken(token);

        return Task.FromResult((JwtToken: jwtToken, ExpireDate: token.ValidTo));
    }
}
