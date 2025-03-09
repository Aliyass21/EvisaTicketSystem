using System;
using System.Text.Json.Serialization;
using EVisaTicketSystem.Core.Data;

namespace EVisaTicketSystem.Core.DTOs
{
    public class TicketAttachmentDto
    {
        public required string FilePath { get; set; }
        public required Guid TicketId { get; set; }
    }
}
