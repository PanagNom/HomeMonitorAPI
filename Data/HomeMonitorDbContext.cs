using HomeMonitorAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeMonitorAPI.Data
{
    public class HomeMonitorDbContext : IdentityDbContext<ApplicationUser>
    {
        public HomeMonitorDbContext(DbContextOptions<HomeMonitorDbContext> options) : base(options)
        {
        }
        public DbSet<Sensor> Sensors { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("refresh_tokens");
            builder.HasKey(e => e.Token);

            builder.Property(e => e.JwtId).IsRequired();
            builder.Property(e => e.ExpiryDate).IsRequired();
            builder.Property(e => e.Invalidated).IsRequired();
            builder.Property(e => e.UserId).IsRequired();
            builder.Property(e => e.CreatedAtUtc).IsRequired();
            builder.Property(e => e.UpdatedAtUtc);

            builder.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId);
        }
    }
}
