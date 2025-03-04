using System;

namespace EVisaTicketSystem.Core.DTOs;

public class UserDto
{
    public required string Username { get; set; }
    public required string FullName { get; set; }

    public required string Token { get; set; }


}

