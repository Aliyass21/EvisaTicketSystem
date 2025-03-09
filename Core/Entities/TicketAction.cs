using System;
using System.Text.Json.Serialization;
using EVisaTicketSystem.Core.Data;
using EVisaTicketSystem.Core.Enums;

namespace EVisaTicketSystem.Core.Entities;

public class TicketAction :Entity
    {
        public required Guid TicketId { get; set; }
        public required Guid UserId { get; set; }
        public required TicketActionType ActionType { get; set; }
        public required DateTime ActionDate { get; set; } = DateTime.UtcNow;

        public string? Notes { get; set; }

        public TicketStatus? PreviousStatus { get; set; }
        
        public TicketStatus? NewStatus { get; set; }
        
        public TicketStage? PreviousStage { get; set; }
        
        public TicketStage? NewStage { get; set; }
        [JsonIgnore] // Prevent circular reference
        public Ticket Ticket { get; set; } = null!;
        [JsonIgnore] // Prevent circular reference
        public AppUser User { get; set; } = null!;
    }
