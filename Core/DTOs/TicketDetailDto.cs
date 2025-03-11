using System;
using System.Collections.Generic;
using EVisaTicketSystem.Core.Enums;

namespace EVisaTicketSystem.Core.DTOs
{
    public class TicketDetailDto
    {
        // Basic Ticket Info
        public Guid Id { get; set; }
        public string? TicketNumber { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public TicketStatus Status { get; set; }
        public TicketStage CurrentStage { get; set; }
        public TicketPriority Priority { get; set; }
        
        // Related Entities
        public Guid TicketTypeId { get; set; }
        public string? TicketTypeName { get; set; }
        
        public Guid CreatedById { get; set; }
        public string? CreatedByName { get; set; }
        
        public Guid? AssignedToId { get; set; }
        public string? AssignedToName { get; set; }
        
        public Guid? OfficeId { get; set; }
        public string? OfficeName { get; set; }
        
        // Timestamps
        public DateTime CreatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        
        // Ticket Actions - Focusing on the ticket actions as specified
        public List<TicketActionDto> Actions { get; set; } = new List<TicketActionDto>();
    }

    public class TicketActionDto
    {
        public Guid Id { get; set; }
        public Guid TicketId { get; set; }
        public Guid UserId { get; set; }
        public string? UserName { get; set; }
        public TicketActionType ActionType { get; set; }
        public DateTime ActionDate { get; set; }
        public string? Notes { get; set; }
        
        // Status and stage changes
        public TicketStatus? PreviousStatus { get; set; }
        public TicketStatus? NewStatus { get; set; }
        public TicketStage? PreviousStage { get; set; }
        public TicketStage? NewStage { get; set; }
    }
}