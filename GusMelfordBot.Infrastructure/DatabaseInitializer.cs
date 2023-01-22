using GusMelfordBot.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace GusMelfordBot.Infrastructure;

public static class DatabaseInitializer
{
    public static void InitRoles(this ModelBuilder modelBuilder)
    {
        Role adminRole = new Role { Name = "Admin" };
        Role userRole = new Role { Name = "User" };
        
        modelBuilder.Entity<Role>().HasData(new List<Role>{ adminRole, userRole});
    }

    public static void InitFeatures(this ModelBuilder modelBuilder)
    {
        Feature abyssFeature = new Feature { Name = "Abyss" };
        
        modelBuilder.Entity<Feature>().HasData(new List<Feature>{ abyssFeature });
    }
}