using EVisaTicketSystem.Core.Data;
using EVisaTicketSystem.Core.DTOs;
using EVisaTicketSystem.Core.Enums;
using EVisaTicketSystem.Specifcation;
using EVisaTicketSystem.Specifcation.Tickets;
using System.Threading.Tasks;

namespace EVisaTicketSystem.Core.Interfaces
{
    public interface ITicketService
    {
        Task<Ticket> CreateTicketAsync(TicketCreateDto ticketDto);
        Task<TicketDetailDto> GetTicketByIdAsync(Guid id);
        Task<IEnumerable<Ticket>> GetAllTicketsAsync();
        Task UpdateTicketAsync(Guid ticketId, TicketActionType actionType, string notes = "");
        Task DeleteTicketAsync(Guid id);
        Task ApproveTicketAsync(Guid ticketId, string notes);
        Task<Ticket> UpdateTicketDetailsAsync(Guid ticketId, TicketUpdateDto updateDto);
        Task<(IEnumerable<Ticket> Items, int TotalCount)> SearchTicketsAsync(FilterTicketsSpecification spec);
        Task<IEnumerable<TicketDetailDto>> GetLastThreeTicketsAsync(Guid userId);
        Task<TicketSummaryDto> GetTicketSummaryForLast7DaysAsync();




    }
}