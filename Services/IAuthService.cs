using HomeMonitorAPI.Models;

namespace HomeMonitorAPI.Services
{
    public interface IAuthService
    {
        Task<RegistrationResponse> Registration(RegistrationRequest registrationRequest, string role);
        Task<LoginResponse> Login(LoginRequest loginRequest);
        Task<RefreshResponse> Refresh(RefreshRequest refreshRequest);
    }
}
