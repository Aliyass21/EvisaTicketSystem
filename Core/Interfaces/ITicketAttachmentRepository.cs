using EVisaTicketSystem.Core.Entities;

namespace EVisaTicketSystem.Core.Interfaces
{
    public interface ITicketAttachmentRepository
    {
        Task<TicketAttachment> CreateAsync(TicketAttachment attachment);
        Task<TicketAttachment?> GetByIdAsync(Guid id);
        Task<IEnumerable<TicketAttachment>> GetAllAsync();
        Task<IEnumerable<TicketAttachment>> GetByTicketIdAsync(Guid ticketId);
        void Delete(TicketAttachment attachment);
    }
}
