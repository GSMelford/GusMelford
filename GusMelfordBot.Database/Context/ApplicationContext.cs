using System;
using GusMelfordBot.DAL.Applications.ContentCollector;
using Microsoft.Extensions.Logging;

namespace GusMelfordBot.Database.Context;

using DAL;
using Microsoft.EntityFrameworkCore;
using Settings;
    
public sealed class ApplicationContext : DbContext
{
    private readonly DatabaseSettings _databaseSettings;

    public DbSet<User> Users { get; set; }
    public DbSet<Chat> Chats { get; set; }
    public DbSet<Content> Content { get; set; }

    public ApplicationContext(
        DatabaseSettings databaseSettings, 
        ILogger<ApplicationContext> logger)
    {
        _databaseSettings = databaseSettings;
        try
        {
            Database.EnsureCreated();
        }
        catch (Exception e)
        {
            logger.LogError("ERROR Connect to Database. Message: {Message} StackTrace: {StackTrace}", 
                e.Message, e.StackTrace);
            throw;
        }
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
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Content>()
            .Property(x => x.Number)
            .ValueGeneratedOnAdd();
    }
}