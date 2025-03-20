namespace EVisaTicketSystem.Core.DTOs
{
    public class TicketSummaryDto
    {
        public int NewTickets { get; set; }
        public int ClosedTickets { get; set; }
        public int InProgressTickets { get; set; }
    }
}
