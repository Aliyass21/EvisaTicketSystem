using System;
using System.Linq.Expressions;
using EVisaTicketSystem.Core.Data;
using EVisaTicketSystem.Core.Enums;
using EVisaTicketSystem.Specifcation;

namespace EVisaTicketSystem.Specifcation.Tickets;

public class FilterTicketsSpecification : BaseSpecification<Ticket>
{
    
    public FilterTicketsSpecification(
        string? ticketNumber = null,
        string? title = null,
        Guid? officeId = null,
        Guid? createdById = null, // New parameter
        TicketStatus? status = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int skip = 0,
        int take = 10,
        string? sortBy = null,
        bool isDescending = true)
        : base(x =>
            (string.IsNullOrEmpty(ticketNumber) || x.TicketNumber.Contains(ticketNumber)) &&
            (string.IsNullOrEmpty(title) || x.Title.Contains(title)) &&
            (!officeId.HasValue || x.OfficeId == officeId.Value) &&
            (!createdById.HasValue || x.CreatedById == createdById.Value) && // Added condition

            (!status.HasValue || x.Status == status.Value) &&
            (!startDate.HasValue || x.DateCreated >= startDate.Value) &&
            (!endDate.HasValue || x.DateCreated <= endDate.Value))
    {
        // Include related entities
        AddInclude(x => x.TicketType);
        AddInclude(x => x.AssignedTo);
        AddInclude(x => x.CreatedBy);
        AddInclude(x => x.Office);

        // Apply pagination
        ApplyPaging(skip, take);

        // Apply sorting based on parameter
        switch (sortBy?.ToLower())
        {
            case "ticketnumber":
                if (isDescending)
                    ApplyOrderByDescending(x => x.TicketNumber);
                else
                    ApplyOrderBy(x => x.TicketNumber);
                break;
            case "title":
                if (isDescending)
                    ApplyOrderByDescending(x => x.Title);
                else
                    ApplyOrderBy(x => x.Title);
                break;
            case "status":
                if (isDescending)
                    ApplyOrderByDescending(x => x.Status);
                else
                    ApplyOrderBy(x => x.Status);
                break;
            case "priority":
                if (isDescending)
                    ApplyOrderByDescending(x => x.Priority);
                else
                    ApplyOrderBy(x => x.Priority);
                break;
            default:
                // Default sorting by DateCreated (newest first)
                ApplyOrderByDescending(x => x.DateCreated);
                break;
        }
    }
}