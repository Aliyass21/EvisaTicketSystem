using System;
using AutoMapper;
using EVisaTicketSystem.Core.Entities;
using EVisaTicketSystem.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EVisaTicketSystem.Core.Data;

public class UserRepository(DataContext context) : IUserRepository
{

    public async Task<AppUser?> GetUserByIdAsync(Guid id)
    {
        return await  context.Users.FindAsync(id);
    }

    public async Task<AppUser?> GetUserByUsernameAsync(string username)
    {
        return await  context.Users.SingleOrDefaultAsync(x=>x.UserName == username);
    }

    public async Task<IEnumerable<AppUser>> GetUsersAsync()
    {
        return await context.Users.ToListAsync();
    }

    public void Update(AppUser user)
    {
        context.Entry(user).State = EntityState.Modified;
    }
    
}

