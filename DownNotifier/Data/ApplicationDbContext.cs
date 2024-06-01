using Microsoft.EntityFrameworkCore;
using DownNotifier.Models;

namespace DownNotifier.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<TargetApp> TargetApps { get; set; }
        public DbSet<Notification> Notifications { get; set; }
    }
}
