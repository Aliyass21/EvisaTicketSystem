public class NotificationDto
{
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public Guid UserId { get; set; }
}
