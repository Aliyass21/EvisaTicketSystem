using System;
using EVisaTicketSystem.Core.Data;

namespace EVisaTicketSystem.Core.Entities;

 public class TicketAttachment:Entity
    {
        public string? FileName { get; set; }
        public required string FilePath { get; set; }
        public Guid TicketId { get; set; }
        public Ticket Ticket { get; set; } =null!;

    }
