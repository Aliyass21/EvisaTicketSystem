using EVisaTicketSystem.Core.Enums;

namespace EVisaTicketSystem.Core.Services
{
    public class TicketStateMachine
    {
        public (TicketStatus NextStatus, TicketStage NextStage) GetNextState(
            TicketStatus currentStatus,
            TicketStage currentStage,
            TicketActionType actionType,
            string role)
        {
            switch (role)
            {
                case "ResidenceUser":
                    // When a ticket is created, it starts as New.
                    if (currentStatus == TicketStatus.New && actionType == TicketActionType.Created)
                        return (TicketStatus.New, TicketStage.ResidenceUser);
                    
                    // After editing a returned or rejected ticket,
                    // using StatusChanged, the ticket is sent to InReview.
                    if ((currentStatus == TicketStatus.Returned || currentStatus == TicketStatus.Rejected) &&
                        actionType == TicketActionType.StatusChanged)
                        return (TicketStatus.InReview, TicketStage.SubAdmin);
                    break;

                case "SubAdmin":
                    // For a ticket in InReview, a SubAdmin can:
                    // - Return it to the resident or
                    // - Escalate it to Admin.
                    if (currentStatus == TicketStatus.InReview)
                    {
                        if (actionType == TicketActionType.Returned)
                            return (TicketStatus.Returned, TicketStage.ResidenceUser);
                        if (actionType == TicketActionType.Escalated)
                            return (TicketStatus.Escalated, TicketStage.SystemAdmin);
                    }
                    
                    // Also, if the ticket is in a Rejected state,
                    // SubAdmin can either return it or escalate it.
                    if (currentStatus == TicketStatus.Rejected)
                    {
                        if (actionType == TicketActionType.Returned)
                            return (TicketStatus.Returned, TicketStage.ResidenceUser);
                        if (actionType == TicketActionType.Escalated)
                            return (TicketStatus.Escalated, TicketStage.SystemAdmin);
                    }
                    break;

                case "Admin":
                    // When an escalated ticket is ready for admin work,
                    // Admin sets it to AdminReview using AdminReview action.
                    if (currentStatus == TicketStatus.Escalated &&
                        actionType == TicketActionType.AdminReview)
                        return (TicketStatus.AdminReview, TicketStage.SystemAdmin);
                    
                    // In AdminReview state, Admin can:
                    // - Approve and send the ticket to ScopeSky (Resolved)
                    // - Reject it (Rejected) to return to SubAdmin
                    // - Cancel it (Cancelled)
                    if (currentStatus == TicketStatus.AdminReview)
                    {
                        if (actionType == TicketActionType.Resolved)
                            return (TicketStatus.SentToScopesky, TicketStage.ScopeSky);
                        if (actionType == TicketActionType.Rejected)
                            return (TicketStatus.Rejected, TicketStage.SubAdmin);
                        if (actionType == TicketActionType.Cancelled)
                            return (TicketStatus.Cancelled, TicketStage.SystemAdmin);
                    }
                    
                    // If ticket is in RejectByScopesky state, Admin can:
                    // - Correct it and send it back to ScopeSky (Resolved)
                    // - Or reject it again (Rejected) to return to SubAdmin.
                    if (currentStatus == TicketStatus.RejectByScopesky)
                    {
                        if (actionType == TicketActionType.Resolved)
                            return (TicketStatus.SentToScopesky, TicketStage.ScopeSky);
                        if (actionType == TicketActionType.Rejected)
                            return (TicketStatus.Rejected, TicketStage.SubAdmin);
                    }
                    
                    // After ScopeSky marks a ticket as Resolved, Admin reviews it.
            if (currentStatus == TicketStatus.Resolved &&
            (actionType == TicketActionType.AdminReview || actionType == TicketActionType.StatusChanged))
            return (TicketStatus.ReviewedByAdmin, TicketStage.ScopeSky);

                    break;

                    case "ScopeSky":
                if (currentStatus == TicketStatus.SentToScopesky &&
                    actionType == TicketActionType.InProgress)
                    return (TicketStatus.InProgress, TicketStage.ScopeSky);

                if (currentStatus == TicketStatus.InProgress && currentStage == TicketStage.ScopeSky)
                {
                    if (actionType == TicketActionType.Resolved)
                        return (TicketStatus.Resolved, TicketStage.SystemAdmin);
                    if (actionType == TicketActionType.Rejected)
                        return (TicketStatus.RejectByScopesky, TicketStage.SystemAdmin);
                }

                if (currentStatus == TicketStatus.ReviewedByAdmin &&
                    actionType == TicketActionType.Closed)
                    return (TicketStatus.Closed, TicketStage.ScopeSky);
                break;

            }

            throw new InvalidOperationException(
                $"Invalid transition from {currentStatus} with action {actionType} for role {role}.");
        }
    }
}
