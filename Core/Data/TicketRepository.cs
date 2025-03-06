using EVisaTicketSystem.Core.Data;
using EVisaTicketSystem.Core.DTOs;
using EVisaTicketSystem.Core.Entities;
using EVisaTicketSystem.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EVisaTicketSystem.Infrastructure.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly DataContext _context;

        public TicketRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Ticket>> GetAllAsync()
        {
            return await _context.Tickets.ToListAsync();
        }

        public async Task<Ticket?> GetByIdAsync(Guid id)
        {
            return await _context.Tickets.FindAsync(id);
        }

        // Create using a Ticket entity
        public Task<Ticket> CreateAsync(Ticket ticket)
        {
            _context.Tickets.Add(ticket);
            return Task.FromResult(ticket);
        }

        // Create using a TicketDto
        public Task<Ticket> CreateAsync(TicketDto ticketDto)
        {
            var ticket = new Ticket
            {
                TicketNumber = ticketDto.TicketNumber,
                Title = ticketDto.Title,
                Description = ticketDto.Description,
                Status = ticketDto.Status,
                CurrentStage = ticketDto.CurrentStage,
                TicketTypeId = ticketDto.TicketTypeId,
                CreatedById = ticketDto.CreatedById,
                AssignedToId = ticketDto.AssignedToId,
                OfficeId = ticketDto.OfficeId,
                ClosedAt = ticketDto.ClosedAt
            };

            _context.Tickets.Add(ticket);
            return Task.FromResult(ticket);
        }

        public Task<Ticket> UpdateAsync(Ticket ticket)
        {
            _context.Tickets.Update(ticket);
            return Task.FromResult(ticket);
        }

        public async Task DeleteAsync(Guid id)
        {
            var ticket = await GetByIdAsync(id);
            if (ticket != null)
            {
                _context.Tickets.Remove(ticket);
            }
        }
    }
}
