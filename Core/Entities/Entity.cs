using System;

namespace EVisaTicketSystem.Core.Entities;

public class Entity
{
    public Guid Id { get; set; }
    public DateTime DateCreated { get; set;} = DateTime.UtcNow;

}