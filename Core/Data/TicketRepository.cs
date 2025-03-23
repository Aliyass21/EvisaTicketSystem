using EVisaTicketSystem.Core.Data;
using EVisaTicketSystem.Core.DTOs;
using EVisaTicketSystem.Core.Enums;
using EVisaTicketSystem.Core.Interfaces;
using EVisaTicketSystem.Specifcation;
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
// Implement the specification-based query methods
        public async Task<IEnumerable<Ticket>> ListAsync(ISpecification<Ticket> spec)
        {
            return await ApplySpecification(spec).ToListAsync();
        }

        public async Task<int> CountAsync(ISpecification<Ticket> spec)
        {
            return await ApplySpecification(spec).CountAsync();
        }

        // Helper method to apply specification to the query
        private IQueryable<Ticket> ApplySpecification(ISpecification<Ticket> spec)
        {
            return SpecificationEvaluator<Ticket>.GetQuery(_context.Set<Ticket>().AsQueryable(), spec);
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
        public async Task<Ticket?> GetByIdWithDetailsAsync(Guid id)
{
    return await _context.Tickets
        .Include(t => t.CreatedBy)
        .Include(t => t.AssignedTo)
        .Include(t => t.Office)
        .Include(t => t.TicketType)
        .Include(t => t.Actions)
            .ThenInclude(a => a.User)
        .FirstOrDefaultAsync(t => t.Id == id);
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
public async Task<IEnumerable<TicketDetailDto>> GetLastThreeTicketsAsync(Guid userId)
{
    return await _context.Tickets
        .Where(t => t.CreatedById == userId)
        .OrderByDescending(t => t.DateCreated)
        .Take(3)
        .Select(t => new TicketDetailDto
        {
            Id             = t.Id,
            TicketNumber   = t.TicketNumber,
            Title          = t.Title,
            Description    = t.Description,
            Status         = t.Status,
            CurrentStage   = t.CurrentStage,
            Priority       = t.Priority,
            TicketTypeId   = t.TicketTypeId,
            TicketTypeName = t.TicketType.Title,
            CreatedById    = t.CreatedById,
            CreatedByName  = t.CreatedBy.FullName,
            AssignedToId   = t.AssignedToId,
            AssignedToName = t.AssignedTo != null ? t.AssignedTo.FullName : null,
            OfficeId       = t.OfficeId,
            OfficeName     = t.Office.Title,
            CreatedAt      = t.DateCreated,
            ClosedAt       = t.ClosedAt,
        })
        .ToListAsync();
}
 
public async Task<TicketSummaryDto> GetTicketSummaryForLast7DaysAsync()
{
    var today = DateTime.UtcNow.Date;
    var startDate = today.AddDays(-6);

    var ticketsLast7 = _context.Tickets
        .Where(t => t.DateCreated.Date >= startDate && t.DateCreated.Date <= today);

    var newTickets = await ticketsLast7.CountAsync(t => t.Status == TicketStatus.New);
    var closedTickets = await ticketsLast7.CountAsync(t => t.Status == TicketStatus.Closed);
    var inProgressTickets = await ticketsLast7.CountAsync(t =>
        t.Status != TicketStatus.New &&
        t.Status != TicketStatus.Closed &&
        t.Status != TicketStatus.Cancelled);

    return new TicketSummaryDto
    {
        NewTickets = newTickets,
        ClosedTickets = closedTickets,
        InProgressTickets = inProgressTickets
    };
}



    }
}
