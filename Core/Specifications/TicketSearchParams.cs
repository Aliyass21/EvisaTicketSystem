using EVisaTicketSystem.Core.Enums;

namespace EVisaTicketSystem.Specifcation.Tickets;
public class TicketSearchParams
{
    public string? TicketNumber { get; set; }
    public string? Title { get; set; }
    public Guid? OfficeId { get; set; }
    public TicketStatus? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; }
    public bool IsDescending { get; set; } = true;
}