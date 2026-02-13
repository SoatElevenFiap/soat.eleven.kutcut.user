using Microsoft.AspNetCore.Mvc;
using Soat.Eleven.Kutcut.Users.Application.DTOs.Inputs;
using Soat.Eleven.Kutcut.Users.Application.Interfaces;

namespace Soat.Eleven.Kutcut.Users.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ILoginHandle _loginHandle;

    public AuthController(ILoginHandle loginHandle)
    {
        _loginHandle = loginHandle;
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginInput input)
    {
        try
        {
            var result = await _loginHandle.HandleAsync(input);
            return Ok(result);
        }
        catch (FluentValidation.ValidationException ex)
        {
            return BadRequest(new { errors = ex.Errors.Select(e => e.ErrorMessage) });
        }
        catch (Exception ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }
}
