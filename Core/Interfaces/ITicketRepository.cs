using EVisaTicketSystem.Core.Data;
using EVisaTicketSystem.Core.DTOs;
using EVisaTicketSystem.Core.Entities;
using EVisaTicketSystem.Specifcation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EVisaTicketSystem.Core.Interfaces
{
    public interface ITicketRepository
    {
        Task<IEnumerable<Ticket>> GetAllAsync();
        Task<Ticket?> GetByIdAsync(Guid id);
        Task<Ticket?> GetByIdWithDetailsAsync(Guid id); // New method

        Task<Ticket> CreateAsync(Ticket ticket);
        // Overload to create a Ticket from a DTO
        Task<Ticket> CreateAsync(TicketDto ticketDto);
        Task<Ticket> UpdateAsync(Ticket ticket);
        Task DeleteAsync(Guid id);
        Task<Ticket?> GetLastTicketAsync();
        Task<IEnumerable<Ticket>> ListAsync(ISpecification<Ticket> spec);
        Task<int> CountAsync(ISpecification<Ticket> spec);
        Task<IEnumerable<TicketDetailDto>> GetLastThreeTicketsAsync(Guid userId);
        Task<TicketSummaryDto> GetTicketSummaryForLast7DaysAsync();


    

    }
}
