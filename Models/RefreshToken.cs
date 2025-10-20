using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeMonitorAPI.Models
{
    public class RefreshToken
    {
        public string Token { get; set; } = string.Empty;
        public string JwtId { get; set; } = string.Empty;
        public DateTime ExpiryDate { get; set; }
        public bool Invalidated { get; set; }
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;
    }
}
