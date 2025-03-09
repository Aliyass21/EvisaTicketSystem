using System;
using System.Text.Json.Serialization;
using EVisaTicketSystem.Core.Data;

namespace EVisaTicketSystem.Core.Entities;

 public class TicketAttachment:Entity
    {
        public required string FilePath { get; set; }
        public Guid TicketId { get; set; }
            [JsonIgnore] // Prevents the serializer from navigating back to Ticket

        public Ticket Ticket { get; set; } =null!;

    }
