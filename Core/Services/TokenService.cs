using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EVisaTicketSystem.Core.Entities;
using EVisaTicketSystem.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace EVisaTicketSystem.Services;

public class TokenService(IConfiguration configuration, UserManager<AppUser> userManager) : ITokenService
{
    public async Task<string> CreateToken(AppUser user)
    {
       var tokenKey = configuration["TokenKey"] ?? throw new Exception("Cannot Access Token Key from app settings");
       if(tokenKey.Length < 64) throw new Exception("Your tokenkey needs to be longer");
       var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
       
       if(user.UserName == null) throw new Exception("No Username For User");

       var claims = new List<Claim>
       {
        new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new(ClaimTypes.Name, user.UserName),
        // Add the Position claim 
        new("Position", user.Position)
       };
       
       var roles = await userManager.GetRolesAsync(user);

       claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));


       var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

       var tokenDescriptor = new SecurityTokenDescriptor
       {
        Subject = new ClaimsIdentity(claims),
        Expires = DateTime.UtcNow.AddDays(7),
        SigningCredentials = creds
       };
       
       var tokenHandler = new JwtSecurityTokenHandler();
       var token = tokenHandler.CreateToken(tokenDescriptor);

       return tokenHandler.WriteToken(token);
    }
}