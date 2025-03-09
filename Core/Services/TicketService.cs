using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using EVisaTicketSystem.Core.Data;
using EVisaTicketSystem.Core.DTOs;
using EVisaTicketSystem.Core.Entities;
using EVisaTicketSystem.Core.Enums;
using EVisaTicketSystem.Core.Interfaces;
using Microsoft.AspNetCore.Http;

namespace EVisaTicketSystem.Core.Services
{
    public class TicketService : ITicketService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPhotoService _photoService; // Inject your photo service

        public TicketService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor,IPhotoService photoService)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _photoService = photoService;

        }

        // Helper method to get the current user's ID
        private Guid GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (Guid.TryParse(userIdClaim, out Guid userId))
            {
                return userId;
            }

            throw new InvalidOperationException("User ID not found or invalid.");
        }

        // Helper method to get the current user's role
private string GetUserRole()
{
    var user = _httpContextAccessor.HttpContext?.User;
    if (user == null)
        throw new InvalidOperationException("User not found in HttpContext.");
    
    // Log all claims for debugging purposes (remove in production)
    foreach (var claim in user.Claims)
    {
        Console.WriteLine($"Claim: {claim.Type} = {claim.Value}");
    }

    var roleClaim = user.FindFirst(ClaimTypes.Role)?.Value;
    if (!string.IsNullOrEmpty(roleClaim))
    {
        return roleClaim;
    }

    throw new InvalidOperationException("User role not found.");
}


        // Create a new ticket using a TicketDto
         // Create a new ticket using TicketCreateDto
        public async Task<Ticket> CreateTicketAsync(TicketCreateDto ticketDto)
        {
            if (ticketDto.CreatedById == Guid.Empty)
            {
                ticketDto.CreatedById = GetCurrentUserId();
            }

            // Map properties from the DTO to a new Ticket
            var ticket = new Ticket
            {
                TicketNumber = ticketDto.TicketNumber,
                Title = ticketDto.Title,
                Description = ticketDto.Description,
                Status = ticketDto.Status,
                CurrentStage = ticketDto.CurrentStage,
                Priority = ticketDto.Priority,
                TicketTypeId = ticketDto.TicketTypeId,
                CreatedById = ticketDto.CreatedById,
                AssignedToId = ticketDto.AssignedToId,
                OfficeId = ticketDto.OfficeId,
                ClosedAt = ticketDto.ClosedAt
            };

            // Create the ticket via repository
            await _unitOfWork.TicketRepository.CreateAsync(ticket);

            // Log the Created action
            var createdAction = new TicketAction
            {
                TicketId = ticket.Id,
                UserId = ticketDto.CreatedById,
                ActionType = TicketActionType.Created,
                ActionDate = DateTime.UtcNow,
                Notes = "Ticket created.",
                NewStatus = ticket.Status,
                NewStage = ticket.CurrentStage
            };

            ticket.Actions.Add(createdAction);

            // If an attachment file is provided, process it.
            if (ticketDto.Attachment != null)
            {
                var uploadResult = await _photoService.AddPhotoAsync(ticketDto.Attachment, ticket.Id);
                var attachment = new TicketAttachment
                {
                    TicketId = ticket.Id,
                    FilePath = uploadResult.FilePath
                };

                ticket.Attachments.Add(attachment);
            }

            await _unitOfWork.Complete();
            return ticket;
        }
        // Get ticket by Id
        public async Task<Ticket> GetTicketByIdAsync(Guid id)
        {
            return await _unitOfWork.TicketRepository.GetByIdAsync(id);
        }

        // Get all tickets
        public async Task<IEnumerable<Ticket>> GetAllTicketsAsync()
        {
            return await _unitOfWork.TicketRepository.GetAllAsync();
        }

        // Update ticket (FSM Transition)
public async Task UpdateTicketAsync(Guid ticketId, TicketActionType actionType, string notes = "")
{
    var ticket = await _unitOfWork.TicketRepository.GetByIdAsync(ticketId);

    if (ticket == null) throw new Exception("Ticket not found.");

    var currentUserRole = GetUserRole();
    var currentStage = ticket.CurrentStage;
    var currentStatus = ticket.Status;

    // Validate role and transition
    (TicketStatus newStatus, TicketStage newStage) = GetNextState(currentStatus, currentStage, actionType, currentUserRole);

    // Update ticket state
    ticket.Status = newStatus;
    ticket.CurrentStage = newStage;

    // If the ticket is being closed, set the closed date
    if (newStatus == TicketStatus.Closed)
    {
        ticket.ClosedAt = DateTime.UtcNow;
    }

    // Create the action log
    var ticketAction = new TicketAction
    {
        TicketId = ticketId,
        UserId = GetCurrentUserId(),
        ActionType = actionType,
        ActionDate = DateTime.UtcNow,
        Notes = notes,
        PreviousStatus = currentStatus,
        NewStatus = newStatus,
        PreviousStage = currentStage,
        NewStage = newStage
    };

    // Add the action to the ticket's actions collection
    ticket.Actions.Add(ticketAction);

    // Update the ticket
    _unitOfWork.TicketRepository.UpdateAsync(ticket);
    await _unitOfWork.Complete();
}



        // FSM Transition Rules
internal (TicketStatus, TicketStage) GetNextState(
    TicketStatus currentStatus,
    TicketStage currentStage,
    TicketActionType actionType,
    string role)
{
    // Transition: New → InReview (Submission by ResidenceUser)
    if (currentStatus == TicketStatus.New && role == "SubAdmin" && actionType == TicketActionType.StatusChanged)
    {
        return (TicketStatus.InReview, TicketStage.SubAdmin);
    }

    // Transition: InReview → Escalated (Escalation by SubAdmin)
    if (currentStatus == TicketStatus.InReview && role == "SubAdmin" && actionType == TicketActionType.Escalated)
    {
        return (TicketStatus.Escalated, TicketStage.SystemAdmin);
    }

    // Transition: InReview → Returned (Ticket is sent back for corrections)
    if (currentStatus == TicketStatus.InReview && role == "SubAdmin" && actionType == TicketActionType.Returned)
    {
        return (TicketStatus.Returned, TicketStage.ResidenceUser);
    }

    // Transition: Escalated → InProgress / Resolved / Rejected (Action by SystemAdmin)
    if (currentStatus == TicketStatus.Escalated && role == "Admin")
    {
        switch (actionType)
        {
            case TicketActionType.InProgress:
                return (TicketStatus.InProgress, TicketStage.SystemAdmin);
            case TicketActionType.Resolved:
                return (TicketStatus.Resolved, TicketStage.ScopeSky);
            case TicketActionType.Rejected:
                return (TicketStatus.Rejected, TicketStage.SubAdmin);
            default:
                throw new InvalidOperationException("Invalid action for Escalated state.");
        }
    }

    // Transition: InProgress → Resolved / Rejected / Cancelled (Action by SystemAdmin)
    if (currentStatus == TicketStatus.InProgress && role == "Admin")
    {
        switch (actionType)
        {
            case TicketActionType.Resolved:
                return (TicketStatus.Resolved, TicketStage.ScopeSky);
            case TicketActionType.Rejected:
                return (TicketStatus.Rejected, TicketStage.SubAdmin);
            case TicketActionType.Cancelled:
                return (TicketStatus.Cancelled, currentStage); // Stage remains unchanged
            default:
                throw new InvalidOperationException("Invalid action for InProgress state.");
        }
    }

    // Transition: Rejected → InReview (Resubmission by ResidenceUser)
    if (currentStatus == TicketStatus.Rejected && role == "SubAdmin" && actionType == TicketActionType.StatusChanged)
    {
        return (TicketStatus.InReview, TicketStage.SubAdmin);
    }

    // Transition: Returned → InReview (Resubmission by ResidenceUser)
    if (currentStatus == TicketStatus.Returned && role == "SubAdmin" && actionType == TicketActionType.StatusChanged)
    {
        return (TicketStatus.InReview, TicketStage.SubAdmin);
    }

    // Transition: Resolved → Closed (Closing by ScopeSky)
    if (currentStatus == TicketStatus.Resolved && role == "ScopeSky" && actionType == TicketActionType.Closed)
    {
        return (TicketStatus.Closed, TicketStage.ScopeSky);
    }

    throw new InvalidOperationException("Invalid transition.");
}


    }
}
