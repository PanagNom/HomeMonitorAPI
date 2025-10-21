using HomeMonitorAPI.Data.Interfaces;
using HomeMonitorAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeMonitorAPI.Data.Repositories
{
    public class AuthenticationRepository: IAuthenticationRepository
    {
        private readonly HomeMonitorDbContext _context;

        public AuthenticationRepository(HomeMonitorDbContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken?> GetRefreshToken(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(nameof(userId), "User cannot be null or empty.");
            }
            var refreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.UserId == userId && rt.Invalidated == false);
            
            if(refreshToken?.ExpiryDate < DateTime.UtcNow)
            {
                refreshToken.Invalidated = true;
                _context.RefreshTokens.Update(refreshToken);
                _context.SaveChanges();
                return null;
            }

            return refreshToken;
        }
        public async Task<string?> AddRefreshToken(string userId, string jti)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId));

            var refreshToken = new Models.RefreshToken
            {
                UserId = userId,
                JwtId = jti,
                Token = Guid.NewGuid().ToString(),
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                Invalidated = false
            };

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return refreshToken.Token;
        }
        async Task IAuthenticationRepository.DeleteRefreshToken(string userId)
        {
            var refreshToken = await GetRefreshToken(userId);
            if (refreshToken is null)
            {
                return ;
            }
            _context.RefreshTokens.Remove(refreshToken);
            _context.SaveChanges();
        }
    }
}
