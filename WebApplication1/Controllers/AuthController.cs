using CcStore.Models;
using CcStore.Repository.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CcStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public AuthController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // POST: api/auth/add
        [HttpPost("add")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (registerDto == null)
                {
                    return BadRequest("User data is required.");
                }

                User user = new()
                {
                    Username = registerDto.Username,
                    Email = registerDto.Email,
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    Gender = registerDto.Gender,
                    Address = registerDto.Address,
                    PasswordHash = registerDto.Password
                };

                var jwtToken = await _userRepository.CreateUserAsync(user);

                // Return response including user data and tokens
                var response = new
                {
                    user.Id,
                    user.Username,
                    user.Email,
                    user.FirstName,
                    user.LastName,
                    user.Gender,
                    user.Image,
                    AccessToken = jwtToken,  // Access token returned
                    RefreshToken = user.RefreshToken  // Refresh token
                };

                return Ok(response);  // Successfully registered
            }
            catch (Exception ex)
            {
                return BadRequest($"Error occurred during registration: {ex.Message}");
            }
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            try
            {
                if (loginRequest == null)
                {
                    return BadRequest("Username and password are required.");
                }

                var jwtToken = await _userRepository.Login(loginRequest.Username, loginRequest.Password);
                var user = await _userRepository.GetUserByUsernameOrEmailAsync(loginRequest.Username);  // Assuming username and email are the same.

                if (user == null)
                {
                    return Unauthorized("Invalid credentials");
                }

                // Return response including user data and tokens
                var response = new
                {
                    user.Id,
                    user.Username,
                    user.Email,
                    user.FirstName,
                    user.LastName,
                    user.Gender,
                    user.Image,
                    AccessToken = jwtToken,  // Access token returned
                    RefreshToken = user.RefreshToken  // Refresh token
                };

                return Ok(response);  // Successfully logged in
            }
            catch (Exception ex)
            {
                return BadRequest($"Error occurred during login: {ex.Message}");
            }
        }
    }

    // Helper model for login request
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
