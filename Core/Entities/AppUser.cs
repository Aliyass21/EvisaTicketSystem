using System;
using Microsoft.AspNetCore.Identity;

namespace EVisaTicketSystem.Core.Entities;

public class AppUser : IdentityUser<Guid>
{
    public required string FullName { get; set; }
    public required string Position { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime LastActive { get; set; } = DateTime.UtcNow;
    public ICollection<AppUserRole> UserRoles { get; set; } = [];
    

}

