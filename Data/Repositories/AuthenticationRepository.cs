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

        public async Task<RefreshToken?> GetRefreshToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException(nameof(token), "token cannot be null or empty.");
            }
            var refreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == token && rt.Invalidated == false);
            
            if(refreshToken?.ExpiryDate < DateTime.UtcNow)
            {
                refreshToken.Invalidated = true;
                _context.RefreshTokens.Update(refreshToken);
                _context.SaveChanges();
                return null;
            }

            return refreshToken;
        }
        async Task IAuthenticationRepository.DeleteRefreshToken(RefreshToken token)
        {
            var refreshToken = await GetRefreshToken(token.UserId);
            if (refreshToken is null)
            {
                return ;
            }
            _context.RefreshTokens.Remove(refreshToken);
            _context.SaveChanges();
        }
        public async Task<string?> AddRefreshToken(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }
            var refreshToken = new Models.RefreshToken
            {
                UserId = userId,
                Token = Guid.NewGuid().ToString(),
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                Invalidated = false
            };
            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return refreshToken.Token;
        }
    }
}
