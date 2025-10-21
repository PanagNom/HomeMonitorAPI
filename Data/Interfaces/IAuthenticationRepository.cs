using HomeMonitorAPI.Models;

namespace HomeMonitorAPI.Data.Interfaces
{
    public interface IAuthenticationRepository
    {
        Task<RefreshToken?> GetRefreshToken(string userId);
        Task<string?> AddRefreshToken(string userId, string jti);
        Task DeleteRefreshToken(string userId);
    }
}
