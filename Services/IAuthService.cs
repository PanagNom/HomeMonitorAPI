using HomeMonitorAPI.Models;

namespace HomeMonitorAPI.Services
{
    public interface IAuthService
    {
        Task<RegistrationResponse> Registration(RegistrationRequest model, string role);
        Task<LoginResponse> Login(LoginRequest model);
        Task<(int, string)> Refresh(String userId);
    }
}
