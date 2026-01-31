using HomeMonitorAPI.Data.Interfaces;
using HomeMonitorAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeMonitorAPI.Data.Repositories
{
    public class AuthenticationRepository(HomeMonitorDbContext context) : IAuthenticationRepository
    {
        private readonly HomeMonitorDbContext context = context;

        public async Task<RefreshToken?> GetRefreshToken(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(nameof(userId), "User cannot be null or empty.");
            }

            var refreshToken = await this.context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.UserId == userId && rt.Invalidated == false);

            if (refreshToken?.ExpiryDate < DateTime.UtcNow)
            {
                refreshToken.Invalidated = true;
                this.context.RefreshTokens.Update(refreshToken);
                this.context.SaveChanges();
                return null;
            }

            return refreshToken;
        }

        public async Task<string?> AddRefreshToken(string userId, string jti)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            var refreshToken = new Models.RefreshToken
            {
                UserId = userId,
                JwtId = jti,
                Token = Guid.NewGuid().ToString(),
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                Invalidated = false,
            };

            await this.context.RefreshTokens.AddAsync(refreshToken);
            await this.context.SaveChangesAsync();

            return refreshToken.Token;
        }

        async Task IAuthenticationRepository.DeleteRefreshToken(string userId)
        {
            var refreshToken = await this.GetRefreshToken(userId);
            if (refreshToken is null)
            {
                return;
            }

            this.context.RefreshTokens.Remove(refreshToken);
            this.context.SaveChanges();
        }
    }
}