using System;
using EVisaTicketSystem.Core.Entities;

namespace EVisaTicketSystem.Interfaces;

public interface IUserRepository
{
    void Update(AppUser user);
    Task<IEnumerable<AppUser>> GetUsersAsync();
    Task<AppUser?> GetUserByIdAsync(Guid id);
    Task<AppUser?> GetUserByUsernameAsync(string username);


}

