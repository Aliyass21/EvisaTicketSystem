public class TicketUpdateDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public Guid? TicketTypeId { get; set; }
    public Guid? OfficeId { get; set; }
    public IFormFile? Attachment { get; set; } // Optional image/attachment
}
