using System;
using EVisaTicketSystem.Core.Entities;

namespace EVisaTicketSystem.Interfaces;

public interface ITokenService
{
    Task<string> CreateToken(AppUser user);

}
