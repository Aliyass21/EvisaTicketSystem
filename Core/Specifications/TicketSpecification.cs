using System;
using EVisaTicketSystem.Core.Data;

namespace EVisaTicketSystem.Specifcation;

public class FilterTicket : BaseSpecification<Ticket>
{
    public FilterTicket(
        string? Title = null,
        string? TicketNumber = null,
        Guid? officeId = null)
        : base(x =>
            (string.IsNullOrEmpty(Title) || x.Title.Contains(Title)) &&
            (string.IsNullOrEmpty(TicketNumber) || x.TicketNumber.Contains(TicketNumber)) &&

            (!officeId.HasValue || x.OfficeId == officeId.Value))
    {
        AddInclude(x => x.Office);
                   // Apply ordering
         ApplyOrderByDescending(x => x.DateCreated);
    }
}
