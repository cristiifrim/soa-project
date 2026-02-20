using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Soa.Contracts.Dtos;
using Soa.UserService.Exceptions;
using Soa.UserService.Services;

namespace Soa.UserService.Controllers;

[ApiController]
[Route("users")]
public class UsersController : ControllerBase
{
    private readonly Soa.UserService.Services.UserService _userService;

    public UsersController(Soa.UserService.Services.UserService userService)
    {
        _userService = userService;
    }

    [HttpPost("signup")]
    public async Task<IActionResult> Signup([FromBody] CreateUserDto dto)
    {
        try
        {
            var result = await _userService.SignupAsync(dto);
            return Ok(result);
        }
        catch (UserServiceException ex)
        {
            return BadRequest(new { type = ex.Type, message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] CreateUserDto dto)
    {
        try
        {
            var result = await _userService.LoginAsync(dto);
            return Ok(result);
        }
        catch (UserServiceException ex)
        {
            return BadRequest(new { type = ex.Type, message = ex.Message });
        }
    }

    [Authorize]
    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOne(string id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user is null) return NotFound();
        return Ok(user);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateUserDto dto)
    {
        var user = await _userService.UpdateAsync(id, dto);
        if (user is null) return NotFound();
        return Ok(user);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var user = await _userService.DeleteAsync(id);
        if (user is null) return NotFound();
        return Ok(user);
    }
}
