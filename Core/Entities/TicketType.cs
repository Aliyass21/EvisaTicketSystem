using System;

namespace EVisaTicketSystem.Core.Entities;

public class TicketType : Entity
{
    public required string Title { get; set; }
    public string? Description { get; set; }

}
