using HomeMonitorAPI.Models;

namespace HomeMonitorAPI.Services
{
    public interface IAuthService
    {
        Task<(int, Registration)> Registration(Registration model, string role);
        Task<(int, Login)> Login(Login model);
        Task<(int, string)> Refresh(String userId);
    }
}
