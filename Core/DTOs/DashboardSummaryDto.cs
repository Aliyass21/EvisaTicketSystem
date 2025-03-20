namespace EVisaTicketSystem.Core.DTOs
{
    public class DashboardSummaryDto
    {
        public int TotalTickets { get; set; }
        public int ClosedTicketsThisMonth { get; set; }
        public int TotalUsers { get; set; }
    }
}
