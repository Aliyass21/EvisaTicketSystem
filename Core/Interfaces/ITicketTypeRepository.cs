using EVisaTicketSystem.Core.DTOs;
using EVisaTicketSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EVisaTicketSystem.Core.Interfaces
{
    public interface ITicketTypeRepository
    {
        Task<IEnumerable<TicketType>> GetAllAsync();
        Task<TicketType?> GetByIdAsync(Guid id);
        Task<TicketType> CreateAsync(TicketType ticketType);
        // New overload accepting a DTO
        Task<TicketType> CreateAsync(TicketTypeDto ticketTypeDto);
        Task<TicketType> UpdateAsync(TicketType ticketType);
        Task DeleteAsync(Guid id);
    }
}
