using System;

namespace EVisaTicketSystem.Core.Entities;

public class Notification : Entity
{
    public required string Message { get; set; }
    public required bool IsRead { get; set; }
    public required Guid UserId { get; set; }
    public AppUser User { get; set; } = null!;
}
