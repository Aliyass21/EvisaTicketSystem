using System;
using System.Text.Json;
using EVisaTicketSystem.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EVisaTicketSystem.Core.Data;

public class Seed
{
    public static async Task SeedUsers(UserManager<AppUser> userManager,RoleManager<AppRole> roleManager)
    {
        if(await userManager.Users.AnyAsync()) return;

        var userData = await File.ReadAllTextAsync("Core/Data/UserSeedData.json");

        var options = new JsonSerializerOptions{PropertyNameCaseInsensitive = true};
        var users = JsonSerializer.Deserialize<List<AppUser>>(userData,options);

        if (users==null) return;

        var roles = new List<AppRole>
        {
            new() {Name = "SubAdmin"},
            new() {Name = "Admin"},
            new() {Name = "ScopeSky"},
        };

        foreach (var role in roles)
        {
            await roleManager.CreateAsync(role);
        }

        foreach (var user in users)
        {
            // user.Photos.First().IsApproved = true;
            user.UserName = user.UserName!.ToLower();
            await userManager.CreateAsync(user, "Admin@123");
            await userManager.AddToRoleAsync(user, "SubAdmin");
        }

        var admin = new AppUser
        {
            UserName = "admin",
            FullName = "Admin",
            Position ="Manager"

        };

        await userManager.CreateAsync(admin, "Admin@123");
        await userManager.AddToRolesAsync(admin, new[] { "Admin", "ScopeSky" });
    }

}

