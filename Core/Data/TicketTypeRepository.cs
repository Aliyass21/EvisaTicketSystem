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
    public class TicketTypeRepository : ITicketTypeRepository
    {
        private readonly DataContext _context;

        public TicketTypeRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TicketType>> GetAllAsync()
        {
            return await _context.TicketTypes.ToListAsync();
        }

        public async Task<TicketType?> GetByIdAsync(Guid id)
        {
            return await _context.TicketTypes.FindAsync(id);
        }

        public Task<TicketType> CreateAsync(TicketType ticketType)
        {
            _context.TicketTypes.Add(ticketType);
            // Do not call _context.SaveChangesAsync() here; let the UnitOfWork handle it.
            return Task.FromResult(ticketType);
        }

        // New method to create a TicketType from a DTO
        public Task<TicketType> CreateAsync(TicketTypeDto ticketTypeDto)
        {
            var ticketType = new TicketType
            {
                // Map DTO properties to the entity.
                // Adjust property names as needed.
                Title = ticketTypeDto.Title,
                Description = ticketTypeDto.Description
            };

            _context.TicketTypes.Add(ticketType);
            return Task.FromResult(ticketType);
        }

        public Task<TicketType> UpdateAsync(TicketType ticketType)
        {
            _context.TicketTypes.Update(ticketType);
            // Do not call _context.SaveChangesAsync() here; let the UnitOfWork handle it.
            return Task.FromResult(ticketType);
        }

        public async Task DeleteAsync(Guid id)
        {
            var ticketType = await GetByIdAsync(id);
            if (ticketType != null)
            {
                _context.TicketTypes.Remove(ticketType);
                // Do not call _context.SaveChangesAsync() here; let the UnitOfWork handle it.
            }
        }
    }
}
