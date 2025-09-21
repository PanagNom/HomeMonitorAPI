using Microsoft.EntityFrameworkCore;

namespace HomeMonitorAPI.Data
{
    public class HomeMonitorDbContext: DbContext
    {
        public HomeMonitorDbContext(DbContextOptions<HomeMonitorDbContext> options) : base(options)
        {
        }
        public DbSet<HomeMonitorAPI.Models.Sensor> Sensors { get; set; }
    }
}
