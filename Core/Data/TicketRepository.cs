using EVisaTicketSystem.Core.Data;
using EVisaTicketSystem.Core.DTOs;
using EVisaTicketSystem.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

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
    return await _context.Tickets
        .Include(t => t.CreatedBy) // Ensure CreatedBy is loaded
        .ToListAsync();
}

  public async Task<Ticket?> GetByIdAsync(Guid id)
{
    return await _context.Tickets
        .Include(t => t.CreatedBy) // Include the CreatedBy navigation property
        .FirstOrDefaultAsync(t => t.Id == id);
}


        // Create using a Ticket entity
        public Task<Ticket> CreateAsync(Ticket ticket)
        {
            _context.Tickets.Add(ticket);
            return Task.FromResult(ticket);
        }
        public async Task<Ticket?> GetLastTicketAsync()
        {
            return await _context.Tickets
                .OrderByDescending(t => t.DateCreated) // Replace with your creation timestamp property
                .FirstOrDefaultAsync();
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
                ClosedAt = ticketDto.ClosedAt,
                Priority = ticketDto.Priority // Make sure TicketDto has this property
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
