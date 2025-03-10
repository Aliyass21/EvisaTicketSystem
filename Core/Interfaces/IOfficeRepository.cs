using EVisaTicketSystem.Core.DTOs;
using EVisaTicketSystem.Core.Entities;
using EVisaTicketSystem.Core.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EVisaTicketSystem.Core.Interfaces
{
    public interface IOfficeRepository
    {
        Task<IEnumerable<Office>> GetAllAsync();
        Task<Office?> GetByIdAsync(Guid id);
        Task<Office> CreateAsync(Office office);
        // New overload to create an Office from a DTO
        Task<Office> CreateAsync(OfficeDto officeDto);
        Task<Office> UpdateAsync(Office office);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<Office>> GetByTypeAsync(OfficeType officeType);

    }
}
