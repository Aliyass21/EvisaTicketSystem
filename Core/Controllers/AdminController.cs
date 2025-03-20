using System;
using EVisaTicketSystem.Core.DTOs;
using EVisaTicketSystem.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EVisaTicketSystem.Core.Controllers;

public class AdminController(UserManager<AppUser> userManager): BaseApiController
{

    [Authorize(Policy ="RequireAdminRole")]
    [HttpGet("users-with-roles")]
    public async Task <ActionResult> GetUsersWithtRoles()
    {
        var users = await userManager.Users
            .OrderBy(x => x.UserName)
            .Select(x => new
            {
                x.Id,
                Username = x.UserName,
                Roles = x.UserRoles.Select(r => r.Role.Name).ToList()
            }).ToListAsync();

        return Ok(users);
    }


    [Authorize(Policy = "RequireAdminRole")]
    [HttpPost("edit-roles/{username}")]
    public async Task<ActionResult> EditRoles(string username, string roles)
    {
        if (string.IsNullOrEmpty(roles)) return BadRequest("you must select at least one role");

        var selectedRoles = roles.Split(",").ToArray();

        var user = await userManager.FindByNameAsync(username);

        if (user == null) return BadRequest("User not found");

        var userRoles = await userManager.GetRolesAsync(user);

        var result = await userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

        if (!result.Succeeded) return BadRequest("Failed to add to roles");

        result = await userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

        if (!result.Succeeded) return BadRequest("Failed to remove from roles");

        return Ok(await userManager.GetRolesAsync(user));
    }
     // **Edit user details (FullName, Position)**
    [HttpPut("edit-user/{id}")]
    public async Task<ActionResult> EditUser(Guid id, [FromBody] EditUserDto model)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user == null) return NotFound("User not found");

        user.FullName = model.FullName;
        user.Position = model.Position;
        user.UserName= model.Username;

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded) return BadRequest("Failed to update user details");

        return Ok(new { message = "User details updated successfully" });
    }

    // **Edit user password**
    [HttpPut("change-password/{id}")]
    public async Task<ActionResult> ChangePassword(Guid id, [FromBody] ChangePasswordDto model)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user == null) return NotFound("User not found");

        var result = await userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
        if (!result.Succeeded) return BadRequest(result.Errors);

        return Ok(new { message = "Password changed successfully" });
    }

    // **Delete user**
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteUser(Guid id)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user == null) return NotFound("User not found");

        var result = await userManager.DeleteAsync(user);
        if (!result.Succeeded) return BadRequest("Failed to delete user");

        return Ok(new { message = "User deleted successfully" });
    }

    

}

