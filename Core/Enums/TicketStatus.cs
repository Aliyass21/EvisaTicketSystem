namespace EVisaTicketSystem.Core.Enums
{
    public enum TicketStatus
    {
        New = 1,
        InReview = 2,
        Returned = 3,
        Escalated = 4,
        InProgress = 5,
        Rejected = 6,
        Cancelled = 7,
        Resolved = 8,
        SentToScopesky = 9,
        ReviewedByAdmin = 10,
        Closed = 11
    }
}