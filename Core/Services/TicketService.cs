using System.Security.Claims;
using EVisaTicketSystem.Core.Data;
using EVisaTicketSystem.Core.Entities;
using EVisaTicketSystem.Core.Enums;
using EVisaTicketSystem.Core.Interfaces;
using Microsoft.AspNetCore.Http;

public class TicketService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TicketService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _httpContextAccessor = httpContextAccessor;
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
        var roleClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;

        if (!string.IsNullOrEmpty(roleClaim))
        {
            return roleClaim;
        }

        throw new InvalidOperationException("User role not found.");
    }

    // FSM Transition Logic
    public async Task UpdateTicketAsync(Guid ticketId, TicketActionType actionType, string notes = "")
    {
        var ticket = await _unitOfWork.TicketRepository.GetByIdAsync(ticketId);

        if (ticket == null) throw new Exception("Ticket not found.");

        var currentUserRole = GetUserRole();
        var currentStage = ticket.CurrentStage;
        var currentStatus = ticket.Status;

        // Validate role and transition
        (TicketStatus newStatus, TicketStage newStage) = GetNextState(currentStatus, currentStage, actionType, currentUserRole);

        // Update ticket
        ticket.Status = newStatus;
        ticket.CurrentStage = newStage;

        // Log action
        var action = new TicketAction
        {
            TicketId = ticketId,
            UserId = GetCurrentUserId(),
            ActionType = actionType,
            ActionDate = DateTime.UtcNow, // Explicitly set ActionDate
            Notes = notes,
            PreviousStatus = currentStatus,
            NewStatus = newStatus,
            PreviousStage = currentStage,
            NewStage = newStage
        };

        // Save changes via Unit of Work
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
    switch (currentStatus)
    {
        // ResidenceUser Stage (New)
        case TicketStatus.New when role == "ResidenceUser" && actionType == TicketActionType.StatusChanged:
            return (TicketStatus.InReview, TicketStage.SubAdmin);

        // SubAdmin Stage (InReview)
        case TicketStatus.InReview when role == "SubAdmin":
            switch (actionType)
            {
                case TicketActionType.Returned:
                    return (TicketStatus.Returned, TicketStage.ResidenceUser);
                case TicketActionType.Escalated:
                    return (TicketStatus.Escalated, TicketStage.SystemAdmin);
                default:
                    throw new InvalidOperationException("Invalid action for SubAdmin.");
            }

        // SystemAdmin Stage (Escalated)
        case TicketStatus.Escalated when role == "SystemAdmin":
            switch (actionType)
            {
                case TicketActionType.InProgress:
                    return (TicketStatus.InProgress, TicketStage.SystemAdmin);
                case TicketActionType.Rejected:
                    return (TicketStatus.Rejected, TicketStage.SubAdmin);
                case TicketActionType.Resolved:
                    return (TicketStatus.Resolved, TicketStage.ScopeSky);
                default:
                    throw new InvalidOperationException("Invalid action for SystemAdmin.");
            }

        // ScopeSky Stage (Resolved)
        case TicketStatus.Resolved when role == "ScopeSky" && actionType == TicketActionType.Closed:
            return (TicketStatus.Closed, TicketStage.ScopeSky);

        // Handle Returned Tickets (ResidenceUser can resubmit)
        case TicketStatus.Returned when role == "ResidenceUser" && actionType == TicketActionType.StatusChanged:
            return (TicketStatus.InReview, TicketStage.SubAdmin);

        // Default: Invalid Transition
        default:
            throw new InvalidOperationException("Invalid transition.");
    }
}
}