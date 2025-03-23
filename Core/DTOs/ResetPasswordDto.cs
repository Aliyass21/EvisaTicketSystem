namespace EVisaTicketSystem.Core.DTOs;

public class ResetPasswordDto
{
    public required string NewPassword { get; set; }
    public required string ConfirmPassword { get; set; }
}
