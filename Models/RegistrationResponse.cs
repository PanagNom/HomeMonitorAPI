using System.ComponentModel.DataAnnotations;

namespace HomeMonitorAPI.Models
{
    public class RegistrationResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public int Status { get; set; }
    }
}
