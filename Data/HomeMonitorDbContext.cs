using HomeMonitorAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HomeMonitorAPI.Data
{
    public class HomeMonitorDbContext : IdentityDbContext<ApplicationUser>
    {
        public HomeMonitorDbContext(DbContextOptions<HomeMonitorDbContext> options) : base(options)
        {
        }
        public DbSet<Sensor> Sensors { get; set; }
    }
}
