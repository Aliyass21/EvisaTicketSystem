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
    public class OfficeRepository : IOfficeRepository
    {
        private readonly DataContext _context;

        public OfficeRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Office>> GetAllAsync()
        {
            return await _context.Offices.ToListAsync();
        }

        public async Task<Office?> GetByIdAsync(Guid id)
        {
            return await _context.Offices.FindAsync(id);
        }

        // Original method accepting an Office entity
        public Task<Office> CreateAsync(Office office)
        {
            _context.Offices.Add(office);
            return Task.FromResult(office);
        }

        // New method accepting an OfficeDto, maps to Office entity
        public Task<Office> CreateAsync(OfficeDto officeDto)
        {
            var office = new Office
            {
                Title = officeDto.Title,
                OfficeType = officeDto.OfficeType
            };
            _context.Offices.Add(office);
            return Task.FromResult(office);
        }

        public Task<Office> UpdateAsync(Office office)
        {
            _context.Offices.Update(office);
            return Task.FromResult(office);
        }

        public async Task DeleteAsync(Guid id)
        {
            var office = await GetByIdAsync(id);
            if (office != null)
            {
                _context.Offices.Remove(office);
            }
        }
    }
}
