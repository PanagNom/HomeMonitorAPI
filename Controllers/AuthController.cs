using System.Text;
using HomeMonitorAPI.Models;
using HomeMonitorAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace HomeMonitorAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController(IConfiguration config, IAuthService authService) : ControllerBase
    {
        private readonly IConfiguration config = config;
        private readonly IAuthService authService = authService;

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            try
            {
                if (!this.ModelState.IsValid)
                {
                    return this.BadRequest("Invalid payload");
                }

                LoginResponse loginResponse = await this.authService.Login(loginRequest);
                if (loginResponse.Status == 0)
                {
                    return this.BadRequest(loginResponse.Message);
                }

                return this.Ok(loginResponse);
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex.Message);
                return this.StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Route("registeration")]
        public async Task<IActionResult> Register(RegistrationRequest registrationRequest)
        {
            try
            {
                if (!this.ModelState.IsValid)
                {
                    return this.BadRequest("Invalid payload");
                }

                RegistrationResponse registrationResponse = await this.authService.Registration(registrationRequest, UserRoles.User);

                if (registrationResponse.Status == 0)
                {
                    return this.BadRequest(registrationResponse.Message);
                }

                return this.CreatedAtAction(nameof(this.Register), registrationResponse);
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex.Message);
                return this.StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Route("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest refreshRequest)
        {
            try
            {
                if (string.IsNullOrEmpty(refreshRequest.UserId))
                {
                    return this.BadRequest("Invalid payload");
                }

                RefreshResponse refreshResponse = await this.authService.Refresh(refreshRequest);
                if (refreshResponse.Status == 0)
                {
                    return this.BadRequest(refreshResponse.Message);
                }

                return this.Ok(refreshResponse);
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex.Message);
                return this.StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}