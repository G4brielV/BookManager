using Microsoft.AspNetCore.Mvc;
using BookManager.API.Services;
using BookManager.API.Services.Impl;
using BookManager.API.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace BookManager.API.Controllers
{
    [Route("[controller]")]
    public class AuthController(IAuthService authService) : Controller
    {

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<RegisterResponse>> Register([FromBody] RegisterRequest request)
        {
            Result<RegisterResponse> result = await authService.RegisterAsync(request);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }

            return CreatedAtAction(nameof(Register), new { id = result.Data.Id }, result.Data);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            Result<LoginResponse> result = await authService.LoginAsync(request);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }
            return Ok(result.Data);
        }
    }
}