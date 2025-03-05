using System;
using System.Security.Claims;

namespace EVisaTicketSystem.Core.Extensions;

public static class ClaimsPrincipleExtensions
{

    public static string GetUserName(this ClaimsPrincipal user) 
    {
        var username = user.FindFirstValue(ClaimTypes.Name) ?? throw new Exception("Cannot get username from token");

        return username;
    }


    public static Guid GetUserId(this ClaimsPrincipal user) 
    {
        return Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    }

}
