using HomeMonitorAPI.Data.Interfaces;
using HomeMonitorAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HomeMonitorAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly IAuthenticationRepository _authRepository;

        public AuthService(UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager, IConfiguration configuration, 
            ILogger logger, IAuthenticationRepository authRepository)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            _configuration = configuration;
            _logger = logger;
            _authRepository = authRepository;
        }
        
        public async Task<RegistrationResponse> Registration(RegistrationRequest registrationRequest, string role)
        {
            RegistrationResponse registrationResponse = new();
            var userExists = await userManager.FindByNameAsync(registrationRequest.Username);
            if (userExists != null)
            {
                registrationResponse.Message = "User already exists";
                registrationResponse.Status = 0;
                return (registrationResponse);
            }

            ApplicationUser user = new()
            {
                Email = registrationRequest.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = registrationRequest.Username,
                FirstName = registrationRequest.FirstName ?? string.Empty,
                LastName = registrationRequest.LastName ?? string.Empty,
            };

            var createUserResult = await userManager.CreateAsync(user, registrationRequest.Password);
            if (!createUserResult.Succeeded)
            {
                registrationResponse.Message = "User creation failed! Please check user details and try again.";
                registrationResponse.Status = 0;

                return (registrationResponse);
            }
                
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));

            if (await roleManager.RoleExistsAsync(role))
                await userManager.AddToRoleAsync(user, role);

            registrationResponse = new()
            {
                Id = user.Id,
                Username = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Message = "User created successfully!",
                Status = 1
            };

            return (registrationResponse);
        }

        public async Task<LoginResponse> Login(LoginRequest loginRequest)
        {
            LoginResponse loginResponse = new();

            var user = await userManager.FindByNameAsync(loginRequest.Username);
            if (user == null)
            {
                loginResponse.Message = "Invalid username";
                loginResponse.Status = 0;
                return (loginResponse);
            }
                
            if (!await userManager.CheckPasswordAsync(user, loginRequest.Password))
            {
                loginResponse.Message = "Invalid password";
                loginResponse.Status = 0;
                return (loginResponse);
            }

            var userRoles = await userManager.GetRolesAsync(user);
            string jti = Guid.NewGuid().ToString();

            var authClaims = new List<Claim>
            {
               new Claim(ClaimTypes.Name, user.UserName!),
               new Claim(JwtRegisteredClaimNames.Jti, jti),
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }
            string token = GenerateToken(authClaims);

            loginResponse = new()
            {
                Id = user.Id,
                JWTID = jti,
                Username = user.UserName?? string.Empty,
                Token = token,
                ExpiryDate = DateTime.UtcNow.AddMinutes(1),
                RefreshToken = await GenerateRefreshToken(user.Id, jti),
                Message = "Login successful",
                Status = 1,
            };
            
            await userManager.UpdateAsync(user);

            return (loginResponse);
        }

        public async Task<(int, string)> Refresh(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return (0, "Invalid user ID");
            }

            var newRefreshToken = await GenerateRefreshToken(userId, "");

            if (string.IsNullOrEmpty(newRefreshToken))
            {
                return (0, "Failed to generate refresh token");
            }
            return (1, newRefreshToken);
        }

        private string GenerateToken(IEnumerable<Claim> claims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]));
            var _TokenExpiryTimeInHour = Convert.ToInt64(_configuration["Jwt:TokenExpiryTimeInHour"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                //Expires = DateTime.UtcNow.AddHours(_TokenExpiryTimeInHour),
                Expires = DateTime.UtcNow.AddMinutes(1),
                SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
                Subject = new ClaimsIdentity(claims)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        
        private async Task<string?> GenerateRefreshToken(string userId, string jti)
        {
            return await _authRepository.AddRefreshToken(userId, jti);
        }

        public async Task<string?> ValidateRefreshToken(string token)
        {
            var refreshToken = await _authRepository.GetRefreshToken(token);
            if (refreshToken is null)
            {
                _logger.LogWarning("Invalid or expired refresh token.");
                return null;
            }
            await _authRepository.DeleteRefreshToken(refreshToken);

            var user = await userManager.FindByIdAsync(refreshToken.UserId);
            if (user is null)
            {
                _logger.LogWarning("User not found for the provided refresh token.");
                return null;
            }
            var userRoles = await userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
            {
               new Claim(ClaimTypes.Name, user.UserName!),
               new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            return GenerateToken(authClaims);
        }
    }
}