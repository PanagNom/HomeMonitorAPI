using HomeMonitorAPI.Models;

namespace HomeMonitorAPI.Services
{
    public interface IAuthService
    {
        Task<(int, string)> Registration(Registration model, string role);
        Task<(int, string)> Login(Login model);
    }
}
