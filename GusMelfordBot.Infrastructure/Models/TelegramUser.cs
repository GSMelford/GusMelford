﻿namespace GusMelfordBot.Infrastructure.Models;

public class TelegramUser : AuditableEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public long TelegramId { get; set; }
}