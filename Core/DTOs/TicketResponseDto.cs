public class TicketResponseDto
{
    public string? TicketNumber { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int Status { get; set; }
    public int CurrentStage { get; set; }
    public int Priority { get; set; }
    public Guid TicketTypeId { get; set; }
    public Guid CreatedById { get; set; }
    public UserSummaryDto? CreatedBy { get; set; }
    public Guid? AssignedToId { get; set; }
    public Guid? OfficeId { get; set; }
    public DateTime? ClosedAt { get; set; }
    public Guid Id { get; set; }
    public DateTime DateCreated { get; set; }
}
