namespace EVisaTicketSystem.Core.DTOs;

public class EditUserDto
{
    public required string FullName { get; set; }
    public required string Username {get;set;}
    public required string Position { get; set; }
}
