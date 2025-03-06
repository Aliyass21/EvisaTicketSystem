using EVisaTicketSystem.Core.Controllers;
using EVisaTicketSystem.Core.DTOs;
using EVisaTicketSystem.Core.Entities;
using EVisaTicketSystem.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EVisaTicketSystem.API.Controllers
{
    public class OfficeController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public OfficeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/Office
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Office>>> GetOffices()
        {
            var offices = await _unitOfWork.OfficeRepository.GetAllAsync();
            return Ok(offices);
        }

        // GET: api/Office/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Office>> GetOffice(Guid id)
        {
            var office = await _unitOfWork.OfficeRepository.GetByIdAsync(id);
            if (office == null)
            {
                return NotFound();
            }
            return Ok(office);
        }

        // POST: api/Office
        [HttpPost]
        public async Task<ActionResult<Office>> CreateOffice([FromBody] OfficeDto officeDto)
        {
            var createdOffice = await _unitOfWork.OfficeRepository.CreateAsync(officeDto);
            await _unitOfWork.Complete(); // Commit changes to the database
            return CreatedAtAction(nameof(GetOffice), new { id = createdOffice.Id }, createdOffice);
        }

        // PUT: api/Office/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOffice(Guid id, [FromBody] OfficeDto officeDto)
        {
            var existingOffice = await _unitOfWork.OfficeRepository.GetByIdAsync(id);
            if (existingOffice == null)
            {
                return NotFound();
            }
            
            // Update the existing office with values from the DTO
            existingOffice.Title = officeDto.Title;
            existingOffice.OfficeType = officeDto.OfficeType;

            await _unitOfWork.OfficeRepository.UpdateAsync(existingOffice);
            await _unitOfWork.Complete(); // Commit changes to the database
            return NoContent();
        }

        // DELETE: api/Office/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOffice(Guid id)
        {
            await _unitOfWork.OfficeRepository.DeleteAsync(id);
            await _unitOfWork.Complete(); // Commit changes to the database
            return NoContent();
        }
    }
}
