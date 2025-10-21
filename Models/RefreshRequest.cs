namespace HomeMonitorAPI.Models
{
    public class RefreshRequest
    {
        public string UserId { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public string JTI { get; set; } = string.Empty;
    }
}
