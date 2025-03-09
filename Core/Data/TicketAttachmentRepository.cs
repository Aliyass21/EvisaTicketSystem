using EVisaTicketSystem.Core.Data;
using EVisaTicketSystem.Core.Entities;
using EVisaTicketSystem.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EVisaTicketSystem.Infrastructure.Repositories
{
    public class TicketAttachmentRepository : ITicketAttachmentRepository
    {
        private readonly DataContext _context;

        public TicketAttachmentRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<TicketAttachment> CreateAsync(TicketAttachment attachment)
        {
            _context.TicketAttachments.Add(attachment);
            // Note: Do not call Complete() here if your unit of work pattern controls saving.
            return attachment;
        }

        public async Task<TicketAttachment?> GetByIdAsync(Guid id)
        {
            return await _context.TicketAttachments
                .Include(ta => ta.Ticket)  // Optional, if you need ticket info
                .FirstOrDefaultAsync(ta => ta.Id == id);
        }

        public async Task<IEnumerable<TicketAttachment>> GetAllAsync()
        {
            return await _context.TicketAttachments
                .Include(ta => ta.Ticket)
                .ToListAsync();
        }

        public async Task<IEnumerable<TicketAttachment>> GetByTicketIdAsync(Guid ticketId)
        {
            return await _context.TicketAttachments
                .Include(ta => ta.Ticket)
                .Where(ta => ta.TicketId == ticketId)
                .ToListAsync();
        }

        public void Delete(TicketAttachment attachment)
        {
            _context.TicketAttachments.Remove(attachment);
        }
        
    }
}
