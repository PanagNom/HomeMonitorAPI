using System.ComponentModel.DataAnnotations;

namespace HomeMonitorAPI.Models
{
    public class LoginResponse
    {
        public string Username { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public int Status { get; set; }
        public string JWTID { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiryDate { get; set; }
    }
}
