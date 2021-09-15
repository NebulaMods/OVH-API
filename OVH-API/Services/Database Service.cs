using Microsoft.EntityFrameworkCore;
using OVHAPI.Database;

namespace OVHAPI.Services
{
    public class DatabaseService : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlite("Data Source=OVH-API.db").UseLazyLoadingProxies();

        public DbSet<LogsSchema.ErrorLogs> Errors { get; set; }
        public DbSet<LogsSchema.AttackLogs> Attacks { get; set; }
        public DbSet<Settings> Setttings { get; set; }
        public DbSet<IPSchema> IPs { get; set; }
    }
}
