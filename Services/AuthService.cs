using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HomeMonitorAPI.Data.Interfaces;
using HomeMonitorAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace HomeMonitorAPI.Services
{
    public class AuthService(UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager, IConfiguration configuration,
        IAuthenticationRepository authRepository) : IAuthService
    {
        private readonly UserManager<ApplicationUser> userManager = userManager;
        private readonly RoleManager<IdentityRole> roleManager = roleManager;
        private readonly IConfiguration configuration = configuration;
        private readonly IAuthenticationRepository authRepository = authRepository;

        public async Task<RegistrationResponse> Registration(RegistrationRequest registrationRequest, string role)
        {
            RegistrationResponse registrationResponse = new ();
            var userExists = await this.userManager.FindByNameAsync(registrationRequest.Username);
            if (userExists != null)
            {
                registrationResponse.Message = "User already exists";
                registrationResponse.Status = 0;
                return registrationResponse;
            }

            ApplicationUser user = new ()
            {
                Email = registrationRequest.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = registrationRequest.Username,
                FirstName = registrationRequest.FirstName ?? string.Empty,
                LastName = registrationRequest.LastName ?? string.Empty,
            };

            var createUserResult = await this.userManager.CreateAsync(user, registrationRequest.Password);
            if (!createUserResult.Succeeded)
            {
                registrationResponse.Message = "User creation failed! Please check user details and try again.";
                registrationResponse.Status = 0;

                return registrationResponse;
            }

            if (!await this.roleManager.RoleExistsAsync(role))
            {
                await this.roleManager.CreateAsync(new IdentityRole(role));
            }

            if (await this.roleManager.RoleExistsAsync(role))
            {
                await this.userManager.AddToRoleAsync(user, role);
            }

            registrationResponse = new ()
            {
                Id = user.Id,
                Username = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Message = "User created successfully!",
                Status = 1,
            };

            return registrationResponse;
        }

        public async Task<LoginResponse> Login(LoginRequest loginRequest)
        {
            LoginResponse loginResponse = new ();

            var user = await this.userManager.FindByNameAsync(loginRequest.Username);
            if (user == null)
            {
                loginResponse.Message = "Invalid username";
                loginResponse.Status = 0;
                return loginResponse;
            }

            if (!await this.userManager.CheckPasswordAsync(user, loginRequest.Password))
            {
                loginResponse.Message = "Invalid password";
                loginResponse.Status = 0;
                return loginResponse;
            }

            var userRoles = await this.userManager.GetRolesAsync(user);
            string jti = Guid.NewGuid().ToString();

            var authClaims = new List<Claim>
            {
               new (ClaimTypes.Name, user.UserName!),
               new (JwtRegisteredClaimNames.Jti, jti),
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            string token = this.GenerateToken(authClaims);

            loginResponse = new ()
            {
                Id = user.Id,
                JWTID = jti,
                Username = user.UserName ?? string.Empty,
                Token = token,
                ExpiryDate = DateTime.UtcNow.AddMinutes(1),
                RefreshToken = await this.GenerateRefreshToken(user.Id, jti),
                Message = "Login successful",
                Status = 1,
            };

            await this.userManager.UpdateAsync(user);

            return loginResponse;
        }

        public async Task<RefreshResponse> Refresh(RefreshRequest refreshRequest)
        {
            RefreshResponse refreshResponse = new ();

            if (refreshRequest is null)
            {
                refreshResponse.Status = 0;
                refreshResponse.Message = "Empty refresh request.";
                return refreshResponse;
            }

            if (!await this.ValidateRefreshToken(refreshRequest))
            {
                refreshResponse.Status = 0;
                refreshResponse.Message = "Invalid refresh token.";
                return refreshResponse;
            }

            var user = await this.userManager.FindByIdAsync(refreshRequest.UserId);

            var userRoles = await this.userManager.GetRolesAsync(user);
            var jti = Guid.NewGuid().ToString();

            if (user is null)
            {
                refreshResponse.Status = 0;
                refreshResponse.Message = "User not found.";
                return refreshResponse;
            }

            var authClaims = new List<Claim>
            {
               new (ClaimTypes.Name, user.UserName!),
               new (JwtRegisteredClaimNames.Jti, jti),
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            refreshResponse.Token = this.GenerateToken(authClaims);
            refreshResponse.RefreshToken = await this.GenerateRefreshToken(user.Id, jti);
            refreshResponse.Status = 1;
            refreshResponse.Message = "Token refreshed successfully.";

            return refreshResponse;
        }

        private string GenerateToken(IEnumerable<Claim> claims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.configuration["Jwt:Secret"]));
            _ = Convert.ToInt64(this.configuration["Jwt:TokenExpiryTimeInHour"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = this.configuration["Jwt:Issuer"],
                Audience = this.configuration["Jwt:Audience"],

                // Expires = DateTime.UtcNow.AddHours(_TokenExpiryTimeInHour),
                Expires = DateTime.UtcNow.AddMinutes(1),
                SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
                Subject = new ClaimsIdentity(claims),
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private async Task<string?> GenerateRefreshToken(string userId, string jti)
        {
            return await this.authRepository.AddRefreshToken(userId, jti);
        }

        private async Task<bool> ValidateRefreshToken(RefreshRequest refreshRequest)
        {
            var refreshToken = await this.authRepository.GetRefreshToken(refreshRequest.UserId);
            if (refreshToken is null)
            {
                return false;
            }

            if (refreshToken.Token != refreshRequest.RefreshToken)
            {
                return false;
            }

            if (refreshToken.JwtId != refreshRequest.JTI)
            {
                return false;
            }

            if (refreshToken.ExpiryDate < DateTime.UtcNow || refreshToken.Invalidated)
            {
                await this.authRepository.DeleteRefreshToken(refreshToken.UserId);
                return false;
            }

            await this.authRepository.DeleteRefreshToken(refreshToken.UserId);
            return true;
        }
    }
}