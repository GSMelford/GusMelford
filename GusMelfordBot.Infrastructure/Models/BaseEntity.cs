using System.ComponentModel.DataAnnotations;

namespace GusMelfordBot.Infrastructure.Models;

public class BaseEntity
{
    [Key] 
    public Guid Id { get; set; } = Guid.NewGuid();
}