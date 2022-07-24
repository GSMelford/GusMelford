using System.ComponentModel.DataAnnotations;

namespace GusMelfordBot.Infrastructure.Models;

public class BaseEntity
{
    [Key] 
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public DateTime ModifiedOn { get; set; } = DateTime.UtcNow;
}