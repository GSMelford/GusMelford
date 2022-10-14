using GusMelfordBot.Infrastructure.Interfaces;
using GusMelfordBot.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace GusMelfordBot.Infrastructure;

public class DatabaseContext : DbContext, IDatabaseContext
{
    private string ConnectionString { get; set; }
    
    public DbSet<Application>? Applications { get; set; }
    public DbSet<Content>? Contents { get; set; }
    public DbSet<Role>? Roles { get; set; }
    public DbSet<TelegramChat>? TelegramChats { get; set; }
    public DbSet<TelegramUser>? TelegramUsers { get; set; }
    public DbSet<User>? Users { get; set; }
    
    public DatabaseContext(DatabaseSettings? databaseSettings = null)
    {
        ConnectionString = BuildConnectionString(databaseSettings ?? new DatabaseSettings
        {
            Host = "db.gusmelford.com",
            Port = 5432,
            Database = "gusmelford.dev",
            Username = string.Empty,
            Password = string.Empty
        });
    }
    
    public async Task InitializeDatabase(DatabaseSettings databaseSettings) 
    {
        ConnectionString = BuildConnectionString(databaseSettings);
        await Database.MigrateAsync();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(ConnectionString);
    }

    public new void Update<TEntity>(TEntity entity)
    {
        if (entity is BaseEntity baseEntity)
        {
            baseEntity.ModifiedOn = DateTime.UtcNow;
        }
    }

    public new void UpdateRange(IEnumerable<object> entities)
    {
        foreach (object entity in entities)
        {
            Update(entity);
        }
    }

    private string BuildConnectionString(DatabaseSettings databaseSettings)
    {
        return $"Host={databaseSettings.Host};" +
               $"Port={databaseSettings.Port};" +
               $"Database={databaseSettings.Database};" +
               $"Username={databaseSettings.Username};" +
               $"Password={databaseSettings.Password};";
    }
}