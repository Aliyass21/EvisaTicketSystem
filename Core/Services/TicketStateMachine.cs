using EVisaTicketSystem.Core.Enums;

namespace EVisaTicketSystem.Core.Services
{
    public class TicketStateMachine
    {
        // FSM Transition Rules
        public (TicketStatus, TicketStage) GetNextState(
            TicketStatus currentStatus,
            TicketStage currentStage,
            TicketActionType actionType,
            string role)
        {
            // Residence User Flow
            if (role == "ResidenceUser")
            {
                // Create new ticket
                if (currentStatus == TicketStatus.New && actionType == TicketActionType.Created)
                {
                    return (TicketStatus.New, TicketStage.ResidenceUser);
                }
                
                // Submit ticket for review
                if (currentStatus == TicketStatus.New && actionType == TicketActionType.StatusChanged)
                {
                    return (TicketStatus.InReview, TicketStage.SubAdmin);
                }
                
                // Resubmit returned ticket
                if (currentStatus == TicketStatus.Returned && actionType == TicketActionType.StatusChanged)
                {
                    return (TicketStatus.InReview, TicketStage.SubAdmin);
                }
                
                // Resubmit rejected ticket
                if (currentStatus == TicketStatus.Rejected && actionType == TicketActionType.StatusChanged)
                {
                    return (TicketStatus.InReview, TicketStage.SubAdmin);
                }
            }
            
            // SubAdmin Flow
            else if (role == "SubAdmin")
            {
                // Create new ticket
                if (currentStatus == TicketStatus.New && actionType == TicketActionType.Created)
                {
                    return (TicketStatus.New, TicketStage.SubAdmin);
                }
                
                // Return ticket to residence user
                if (currentStatus == TicketStatus.InReview && actionType == TicketActionType.Returned)
                {
                    return (TicketStatus.Returned, TicketStage.ResidenceUser);
                }
                
                // Escalate ticket to system admin
                if (currentStatus == TicketStatus.InReview && actionType == TicketActionType.Escalated)
                {
                    return (TicketStatus.Escalated, TicketStage.SystemAdmin);
                }
            }
            
            // SystemAdmin Flow
            else if (role == "Admin")
            {
                // Handle escalated tickets
                if (currentStatus == TicketStatus.Escalated)
                {
                    switch (actionType)
                    {
                        case TicketActionType.InProgress:
                            return (TicketStatus.InProgress, TicketStage.SystemAdmin);
                        case TicketActionType.Resolved:
                            return (TicketStatus.SentToScopesky, TicketStage.ScopeSky);
                        case TicketActionType.Rejected:
                            return (TicketStatus.Rejected, TicketStage.SubAdmin);
                        default:
                            throw new InvalidOperationException("Invalid action for Escalated state.");
                    }
                }
                
                // Handle in-progress tickets (by Admin)
                if (currentStatus == TicketStatus.InProgress && currentStage == TicketStage.SystemAdmin)
                {
                    switch (actionType)
                    {
                        case TicketActionType.Resolved:
                            return (TicketStatus.SentToScopesky, TicketStage.ScopeSky);
                        case TicketActionType.Rejected:
                            return (TicketStatus.Rejected, TicketStage.SubAdmin);
                        case TicketActionType.Cancelled:
                            return (TicketStatus.Cancelled, TicketStage.SystemAdmin);
                        default:
                            throw new InvalidOperationException("Invalid action for InProgress state.");
                    }
                }
                
                // Review resolved tickets
                if (currentStatus == TicketStatus.Resolved && actionType == TicketActionType.StatusChanged)
                {
                    return (TicketStatus.ReviewedByAdmin, TicketStage.ScopeSky);
                }
            }
            
            // ScopeSky Flow
            else if (role == "ScopeSky")
            {
                // Process tickets sent from admin
                if (currentStatus == TicketStatus.SentToScopesky)
                {
                    switch (actionType)
                    {
                        case TicketActionType.InProgress:
                            return (TicketStatus.InProgress, TicketStage.ScopeSky);
                        case TicketActionType.Rejected:
                            return (TicketStatus.Rejected, TicketStage.SystemAdmin);
                        default:
                            throw new InvalidOperationException("Invalid action for SentToScopesky state.");
                    }
                }
                
                // Handle in-progress tickets (by ScopeSky)
                if (currentStatus == TicketStatus.InProgress && currentStage == TicketStage.ScopeSky)
                {
                    switch (actionType)
                    {
                        case TicketActionType.Cancelled:
                            return (TicketStatus.Cancelled, TicketStage.ScopeSky);
                        case TicketActionType.Resolved:
                            return (TicketStatus.Resolved, TicketStage.SystemAdmin);
                        default:
                            throw new InvalidOperationException("Invalid action for InProgress state in ScopeSky.");
                    }
                }
                
                // Close reviewed tickets
                if (currentStatus == TicketStatus.ReviewedByAdmin && actionType == TicketActionType.Closed)
                {
                    return (TicketStatus.Closed, TicketStage.ScopeSky);
                }
            }

            throw new InvalidOperationException($"Invalid transition from {currentStatus} with action {actionType} for role {role}.");
        }
    }
}