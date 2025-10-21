using HomeMonitorAPI.Models;
using HomeMonitorAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace HomeMonitorAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IConfiguration config, IAuthService authService, ILogger<AuthController> logger)
        {
            _config = config;
            _authService = authService;
            _logger = logger;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest("Invalid payload");
                LoginResponse loginResponse = await _authService.Login(loginRequest);
                if (loginResponse.Status == 0)
                    return BadRequest(loginResponse.Message);
                return Ok(loginResponse.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Route("registeration")]
        public async Task<IActionResult> Register(RegistrationRequest registrationRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest("Invalid payload");
                
                RegistrationResponse registrationResponse = await _authService.Registration(registrationRequest, UserRoles.Admin);
                
                if (registrationResponse.Status == 0)
                {
                    return BadRequest(registrationResponse.Message);
                }
                return CreatedAtAction(nameof(Register), registrationRequest);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Route("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest refreshRequest)
        {
            try
            {
                if (string.IsNullOrEmpty(refreshRequest.UserId))
                    return BadRequest("Invalid payload");
                RefreshResponse refreshResponse = await _authService.Refresh(refreshRequest);
                if (refreshResponse.Status == 0)
                    return BadRequest(refreshResponse.Message);
                return Ok(refreshResponse.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

    }
}
