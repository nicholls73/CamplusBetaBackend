using CamplusBetaBackend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;
using CamplusBetaBackend.DTOs;

namespace CamplusBetaBackend.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller {
        private readonly string? ADMIN_API_KEY = Environment.GetEnvironmentVariable("ADMIN_API_KEY");
        private readonly string? RO_API_KEY = Environment.GetEnvironmentVariable("RO_API_KEY");
        private readonly ILogger<UserController> _logger;
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public UserController(ILogger<UserController> logger, IConfiguration configuration, UserManager<User> userManager, SignInManager<User> signInManager) {
            _logger = logger;
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] LoginDetails loginDetails, [FromHeader(Name = "X-API-Key")] string apiKey) {
            if (!IsApiKeyValid(apiKey, true)) {
                return Unauthorized("Invalid API Key");
            }

            var user = new User { UserName = loginDetails.Email, Email = loginDetails.Email };
            var result = await _userManager.CreateAsync(user, loginDetails.Password);

            if (result.Succeeded) {
                return Ok("User registered successfully!");
            } else {
                return BadRequest(result.Errors);
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDetails loginDetails, [FromHeader(Name = "X-API-Key")] string apiKey) {
            if (!IsApiKeyValid(apiKey, true)) {
                return Unauthorized("Invalid API Key");
            }

            SignInResult? result = await _signInManager.PasswordSignInAsync(loginDetails.Email, loginDetails.Password, isPersistent: true, lockoutOnFailure: false);
            UserResponseDTO userResponseDTO = new UserResponseDTO { Successful = false, Message = "" };

            if (result.Succeeded) {
                userResponseDTO.Successful = true;
                userResponseDTO.Message = "User logged in successfully!";

                return Ok(userResponseDTO);
            } else if (result.IsLockedOut) {
                userResponseDTO.Message = "User is locked out.";

                return BadRequest(userResponseDTO);
            } else {
                userResponseDTO.Message = "Invalid login attempt.";

                return BadRequest(userResponseDTO);
            }
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout([FromHeader(Name = "X-API-Key")] string apiKey) {
            if (!IsApiKeyValid(apiKey, true)) {
                return Unauthorized("Invalid API Key");
            }

            await _signInManager.SignOutAsync();
            return Ok();
        }

        [HttpGet("CheckAuth")]
        public IActionResult CheckAuth([FromHeader(Name = "X-API-Key")] string apiKey) {
            if (!IsApiKeyValid(apiKey, true)) {
                return Unauthorized("Invalid API Key");
            }

            UserResponseDTO userResponseDTO = new UserResponseDTO { Successful = false, Message = "" };

            if (User?.Identity?.IsAuthenticated == true) {
                userResponseDTO.Successful = true;
                return Ok(userResponseDTO);
            } else {
                return Ok(userResponseDTO);
            }
        }

        private bool IsApiKeyValid(string apiKey, bool isAdmin) {
            if (isAdmin) {
                return apiKey == ADMIN_API_KEY;
            }
            else {
                return apiKey == ADMIN_API_KEY || apiKey == RO_API_KEY;
            }
        }
    }
}
