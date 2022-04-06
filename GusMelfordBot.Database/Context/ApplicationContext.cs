using GusMelfordBot.DAL.Applications.MemesChat;
using GusMelfordBot.DAL.Applications.MemesChat.TikTok;

namespace GusMelfordBot.Database.Context;

using Settings;
using DAL;
using Microsoft.EntityFrameworkCore;
    
public sealed class ApplicationContext : DbContext
{
    private readonly DatabaseSettings _databaseSettings;

    public DbSet<User> Users { get; set; }
    public DbSet<TikTokVideoContent> TokVideoContents { get; set; }
    public DbSet<MemesChat> MemesChats { get; set; }

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