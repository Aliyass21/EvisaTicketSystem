using System;
using AutoMapper;
using EVisaTicketSystem.Core.DTOs;
using EVisaTicketSystem.Core.Entities;
using EVisaTicketSystem.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EVisaTicketSystem.Core.Controllers;

public class AccountController(UserManager<AppUser> userManager,ITokenService tokenService,IMapper mapper,RoleManager<AppRole> roleManager) : BaseApiController
{
    [Authorize(Policy ="RequireAdminRole")]
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto) 
    {
        if (await ExistUser(registerDto.Username)) 
            return BadRequest("Username is already taken");

        var user = mapper.Map<AppUser>(registerDto);

        var result = await userManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded) 
            return BadRequest(result.Errors);

        // Assign Roles
        if (registerDto.Roles != null && registerDto.Roles.Any())
        {
            foreach (var role in registerDto.Roles)
            {
                var roleExists = await roleManager.RoleExistsAsync(role);
                if (!roleExists)
                    return BadRequest($"Role '{role}' does not exist.");

                await userManager.AddToRoleAsync(user, role);
            }
        }

        return new UserDto
        {
            Username = user.UserName!,
            FullName = user.FullName,
            Token = await tokenService.CreateToken(user),
        };
    }


    [HttpPost("Login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await userManager.Users
        .FirstOrDefaultAsync(x => x.NormalizedUserName == loginDto.UserName.ToUpper());

        if(user == null || user.UserName ==null) return Unauthorized("Invalid Username");
        
        var result = await userManager.CheckPasswordAsync(user,loginDto.Password);

        if(!result) return Unauthorized();

        return new UserDto
        {
            Username = user.UserName,
            FullName = user.FullName,
            Token = await tokenService.CreateToken(user),
        };

    }

    private async Task<bool> ExistUser(string Username){
        
        return await userManager.Users.AnyAsync(x=>x.NormalizedUserName == Username.ToUpper());
    }
}

