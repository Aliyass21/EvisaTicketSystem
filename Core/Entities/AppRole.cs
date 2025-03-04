using System;
using Microsoft.AspNetCore.Identity;

namespace EVisaTicketSystem.Core.Entities;

public class AppRole : IdentityRole<Guid>
{

    public ICollection<AppUserRole> UserRoles { get; set; } = [];
    
}