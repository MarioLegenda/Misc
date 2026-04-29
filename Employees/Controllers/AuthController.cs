using Employees.Services;

namespace Employees.Controllers;

using Employees.Auth;
using Employees.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly SignInManager<User> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<User> _userManager;
    private readonly JwtService _jwtService;

    public AuthController(RoleManager<IdentityRole> roleManager, UserManager<User> userManager, SignInManager<User> signInManager, JwtService jwtService)
    {
        _roleManager = roleManager;
        _signInManager = signInManager;
        _userManager = userManager;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var user = new User
        {
            UserName = dto.Email,
            Email = dto.Email,
            Name = dto.Name,
            LastName = dto.LastName
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
            return BadRequest(result.Errors);
        
        var roleExists = await _roleManager.RoleExistsAsync("User");
        if (!roleExists)
        {
            await _roleManager.CreateAsync(new IdentityRole("User"));
        }

        // optionally assign default role
        await _userManager.AddToRoleAsync(user, "User");
        
        return Created(string.Empty, new
        {
            user.Id,
            user.Email,
            user.Name,
            user.LastName
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            return Unauthorized();

        var result = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (!result)
            return Unauthorized();

        var roles = await _userManager.GetRolesAsync(user);

        var token = _jwtService.GenerateToken(user, roles);

        return Ok(new
        {
            token
        });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok(new { message = "Logged out" });
    }
}