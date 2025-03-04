using System;
using EVisaTicketSystem.Core.Enums;

namespace EVisaTicketSystem.Core.Entities;

public class Office:Entity
{
    public required string Title { get; set; }
    public required OfficeType OfficeType { get; set; }

}
