using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RevisionNotes.StandardApi.Features.Todos;
using RevisionNotes.StandardApi.Security;

namespace RevisionNotes.StandardApi.Controllers;

[ApiController]
[Route("auth")]
public sealed class AuthController(IOptions<JwtIssuerOptions> jwtOptions, JwtTokenFactory tokenFactory, IConfiguration configuration) : ControllerBase
{
    [HttpPost("token")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult IssueToken([FromBody] LoginRequest request)
    {
        var expectedUser = configuration["DemoCredentials:Username"] ?? "demo";
        var expectedPassword = configuration["DemoCredentials:Password"] ?? "ChangeMe!123";

        if (!string.Equals(request.Username, expectedUser, StringComparison.Ordinal) ||
            !string.Equals(request.Password, expectedPassword, StringComparison.Ordinal))
        {
            return Unauthorized();
        }

        var token = tokenFactory.CreateToken(jwtOptions.Value, request.Username);
        return Ok(new { access_token = token, token_type = "Bearer" });
    }
}