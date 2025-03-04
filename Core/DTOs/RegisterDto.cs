using System;
using System.ComponentModel.DataAnnotations;

namespace EVisaTicketSystem.Core.DTOs;

public class RegisterDto 
{
    [Required]
    public string Username { get; set; } = string.Empty;
    
    [Required] 
    public string FullName { get; set; } = string.Empty;

    [Required] 
    public string Position { get; set; } = string.Empty;

    [Required]
    [StringLength(14, MinimumLength = 4)]
    public string Password { get; set; } = string.Empty;

    public List<string> Roles { get; set; } = []; // List of roles to be assigned
}
