using Microsoft.AspNetCore.Identity;

namespace HomeMonitorAPI.Models
{
    public class ApplicationUser: IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public DateTime TokenExpiry { get; set; } 
    }
}
