using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PortfolioApi.Auth;
using PortfolioApi.Data.Entities;
using PortfolioApi.Dtos;

namespace PortfolioApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public AuthController(UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    // POST: api/auth/register
    // Iedere nieuwe gebruiker krijgt de rol Gast. Editor of Admin kan hier nooit gekozen worden.
    [HttpPost("register")]
    public async Task<ActionResult<GebruikerDto>> Register(RegisterDto dto)
    {
        var user = new User { UserName = dto.Email, Email = dto.Email };

        var result = await _userManager.CreateAsync(user, dto.Wachtwoord);
        if (!result.Succeeded)
        {
            // Bijvoorbeeld: wachtwoord te zwak of e-mail al in gebruik
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }
            return ValidationProblem(ModelState);
        }

        await _userManager.AddToRoleAsync(user, Rollen.Gast);

        return Ok(new GebruikerDto { Email = user.Email!, Rollen = new List<string> { Rollen.Gast } });
    }

    // POST: api/auth/login
    // Bij een geslaagde login zet Identity een authenticatiecookie (Set-Cookie).
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var result = await _signInManager.PasswordSignInAsync(
            dto.Email, dto.Wachtwoord, isPersistent: false, lockoutOnFailure: false);

        if (!result.Succeeded)
        {
            return Unauthorized();
        }

        return Ok();
    }

    // POST: api/auth/logout
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return NoContent();
    }

    // GET: api/auth/me
    // Laat zien wie er is ingelogd en met welke rollen
    [Authorize]
    [HttpGet("me")]
    public ActionResult<GebruikerDto> Me()
    {
        return Ok(new GebruikerDto
        {
            Email = User.Identity?.Name ?? string.Empty,
            Rollen = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList()
        });
    }
}
