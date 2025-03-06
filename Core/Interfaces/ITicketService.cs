using EVisaTicketSystem.Core.Data;
using EVisaTicketSystem.Core.DTOs;
using EVisaTicketSystem.Core.Enums;
using System.Threading.Tasks;

namespace EVisaTicketSystem.Core.Interfaces
{
    public interface ITicketService
    {
        Task<Ticket> CreateTicketAsync(TicketDto ticketDto);
        Task<Ticket> GetTicketByIdAsync(Guid id);
        Task<IEnumerable<Ticket>> GetAllTicketsAsync();
        Task UpdateTicketAsync(Guid ticketId, TicketActionType actionType, string notes = "");
    }
}