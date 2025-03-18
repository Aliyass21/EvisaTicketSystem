namespace EVisaTicketSystem.Core.Enums
{
    public enum TicketStatus
    {
        New = 1,
        InReview = 2,
        Returned = 3,
        Escalated = 4,
        AdminReview = 5 ,  // <-- Inserted here (or any position you prefer)

        InProgress = 6,
        Rejected = 7,
        Cancelled = 8,
        Resolved = 9,
        SentToScopesky = 10,
        ReviewedByAdmin = 11,
        Closed = 12,

        // NEW status
    }
}
