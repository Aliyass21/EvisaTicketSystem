using Microsoft.AspNetCore.Http;
using EVisaTicketSystem.Core.Enums;

namespace EVisaTicketSystem.Core.DTOs
{
    public class TicketCreateDto
    {
        //public required string TicketNumber { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public required TicketStatus Status { get; set; }
        public required TicketStage CurrentStage { get; set; }
        public required TicketPriority Priority { get; set; }
        public required Guid TicketTypeId { get; set; }
        public required Guid CreatedById { get; set; }
        public Guid? AssignedToId { get; set; }
        public Guid? OfficeId { get; set; }
        public DateTime? ClosedAt { get; set; }
        
        // Attachment sent as part of the form
        public IFormFile? Attachment { get; set; }
    }
}
