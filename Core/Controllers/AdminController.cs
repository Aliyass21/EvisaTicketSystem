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
[Authorize(Policy = "RequireAdminRole")]
[HttpPut("edit-user/{id}")]
public async Task<ActionResult> EditUser(Guid id, [FromBody] EditUserDto model)
{
    var user = await userManager.FindByIdAsync(id.ToString());
    if (user == null) return NotFound("User not found");

    user.FullName = model.FullName;
    user.Position = model.Position;
    user.UserName = model.Username;

    var updateResult = await userManager.UpdateAsync(user);
    if (!updateResult.Succeeded)
        return BadRequest(updateResult.Errors);

    if (model.Roles != null)
    {
        var currentRoles = await userManager.GetRolesAsync(user);
        var rolesToAdd = model.Roles.Except(currentRoles);
        var rolesToRemove = currentRoles.Except(model.Roles);

        var addResult = await userManager.AddToRolesAsync(user, rolesToAdd);
        if (!addResult.Succeeded)
            return BadRequest(addResult.Errors);

        var removeResult = await userManager.RemoveFromRolesAsync(user, rolesToRemove);
        if (!removeResult.Succeeded)
            return BadRequest(removeResult.Errors);
    }

    var updatedRoles = await userManager.GetRolesAsync(user);
    return Ok(new 
    { 
        message = "User details and roles updated successfully", 
        roles = updatedRoles 
    });
}

    // **Edit user password**
    [HttpPut("change-password/{id}")]
    [Authorize]

    
    public async Task<ActionResult> ChangePassword(Guid id, [FromBody] ChangePasswordDto model)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user == null) return NotFound("User not found");

        var result = await userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
        if (!result.Succeeded) return BadRequest(result.Errors);

        return Ok(new { message = "Password changed successfully" });
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpPut("reset-password/{id}")]
    public async Task<ActionResult> ResetPassword(Guid id, [FromBody] ResetPasswordDto model)
    {
        if (model.NewPassword != model.ConfirmPassword)
            return BadRequest("Passwords do not match.");

        var user = await userManager.FindByIdAsync(id.ToString());
        if (user == null) 
            return NotFound("User not found");

        // Remove existing password
        var removeResult = await userManager.RemovePasswordAsync(user);
        if (!removeResult.Succeeded)
            return BadRequest(removeResult.Errors);
        
        // Add the new password
        var addResult = await userManager.AddPasswordAsync(user, model.NewPassword);
        if (!addResult.Succeeded)
            return BadRequest(addResult.Errors);

        return Ok(new { message = "Password has been reset successfully." });
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

