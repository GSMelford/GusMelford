using GusMelfordBot.Database.Settings;

namespace GusMelfordBot.Database.Context
{
    using DAL;
    using DAL.TikTok;
    using Microsoft.EntityFrameworkCore;
    
    public sealed class ApplicationContext : DbContext
    {
        private readonly DatabaseSettings _databaseSettings;

        public DbSet<User> Users { get; set; }
        public DbSet<Video> Videos { get; set; }
        public DbSet<Comment> Comments { get; set; }

        public ApplicationContext(DatabaseSettings databaseSettings)
        {
            _databaseSettings = databaseSettings;
            Database.EnsureCreated();
        }
 
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(
                $"Host={_databaseSettings.Host};" +
                $"Port={_databaseSettings.Port};" +
                $"Database={_databaseSettings.Database};" +
                $"Username={_databaseSettings.Username};" +
                $"Password={_databaseSettings.Password}");
        }
    }
}