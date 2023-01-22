using GusMelfordBot.Infrastructure.Interfaces;
using GusMelfordBot.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace GusMelfordBot.Infrastructure;

public class DatabaseContext : DbContext, IDatabaseContext
{
    private string ConnectionString { get; set; }
    
    public DbSet<AuthorizationUserDatum>? AuthorizationUserData { get; set; }
    public DbSet<Content>? Contents { get; set; }
    public DbSet<Feature>? Features { get; set; }
    public DbSet<Group>? Groups { get; set; }
    public DbSet<MetaContent>? MetaContents { get; set; }
    public DbSet<Role>? Roles { get; set; }
    public DbSet<TelegramUser>? TelegramUsers { get; set; }
    public DbSet<User>? Users { get; set; }
    public DbSet<UserContentComment>? UserContentComments { get; set; }
    
    
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
    
    public async Task InitializeDatabaseAsync(DatabaseSettings databaseSettings) 
    {
        ConnectionString = BuildConnectionString(databaseSettings);
        await Database.MigrateAsync();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
        modelBuilder.Entity<TelegramUser>().HasIndex(u => u.TelegramId).IsUnique();
        modelBuilder.Entity<Content>().HasIndex(u => u.OriginalLink).IsUnique();
        modelBuilder.Entity<Feature>().HasIndex(u => u.Name).IsUnique();
        modelBuilder.Entity<Role>().HasIndex(u => u.Name).IsUnique();
        
        modelBuilder.InitRoles();
        modelBuilder.InitFeatures();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(ConnectionString);
    }

    public new void Update<TEntity>(TEntity entity)
    {
        if (entity is AuditableEntity baseEntity)
        {
            baseEntity.ModifiedOn = DateTime.UtcNow;
            base.Update(baseEntity);
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