using System;
using EVisaTicketSystem.Core.Entities;
using EVisaTicketSystem.Core.Enums;

namespace EVisaTicketSystem.Core.Data;

public class Ticket :Entity
    {
        public required string  TicketNumber { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public required TicketStatus Status { get; set; }
        public required TicketStage CurrentStage { get; set; }

        public required Guid TicketTypeId { get; set; }
        public required Guid CreatedById { get; set; }
        public Guid? AssignedToId { get; set; }
        public Guid? OfficeId { get; set; }

        public TicketType TicketType { get; set; } = null!;
        public AppUser AssignedTo { get; set; } = null!;
        public AppUser CreatedBy { get; set; } = null!;
        public Office Office { get; set; } = null!;
        public ICollection<TicketAction> Actions  { get; set; } = [];
        public ICollection<TicketAttachment> Attachments { get; set; } =[];

        public DateTime? ClosedAt { get; set; }



    }
