using System;
using EVisaTicketSystem.Core.Enums;

namespace EVisaTicketSystem.Core.DTOs
{
    public class TicketDto
    {
        public required string TicketNumber { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public required TicketStatus Status { get; set; }
        public required TicketStage CurrentStage { get; set; }
        
        public required Guid TicketTypeId { get; set; }
        public required Guid CreatedById { get; set; }
        public Guid? AssignedToId { get; set; }
        public Guid? OfficeId { get; set; }
        public DateTime? ClosedAt { get; set; }
        
        // Adding the Priority property with a default value of Low
        public TicketPriority Priority { get; set; } = TicketPriority.Low;
    }
}
