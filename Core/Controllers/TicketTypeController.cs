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
    public class TicketTypeController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public TicketTypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/TicketType
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TicketType>>> GetTicketTypes()
        {
            var ticketTypes = await _unitOfWork.TicketTypeRepository.GetAllAsync();
            return Ok(ticketTypes);
        }

        // GET: api/TicketType/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<TicketType>> GetTicketType(Guid id)
        {
            var ticketType = await _unitOfWork.TicketTypeRepository.GetByIdAsync(id);
            if (ticketType == null)
            {
                return NotFound();
            }
            return Ok(ticketType);
        }

        // POST: api/TicketType
        [HttpPost]
        public async Task<ActionResult<TicketType>> CreateTicketType([FromBody] TicketTypeDto ticketTypeDto)
        {
            var createdTicketType = await _unitOfWork.TicketTypeRepository.CreateAsync(ticketTypeDto);
            // Optionally, you can call _unitOfWork.Complete() if you want to commit changes via the unit of work.
            return CreatedAtAction(nameof(GetTicketType), new { id = createdTicketType.Id }, createdTicketType);
        }

        // PUT: api/TicketType/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTicketType(Guid id, [FromBody] TicketTypeDto ticketTypeDto)
        {
            var existingTicketType = await _unitOfWork.TicketTypeRepository.GetByIdAsync(id);
            if (existingTicketType == null)
            {
                return NotFound();
            }

            // Update properties from the DTO.
            existingTicketType.Title = ticketTypeDto.Title;
            existingTicketType.Description = ticketTypeDto.Description;

            await _unitOfWork.TicketTypeRepository.UpdateAsync(existingTicketType);
            // Optionally, call _unitOfWork.Complete() here if needed.
            return NoContent();
        }

        // DELETE: api/TicketType/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTicketType(Guid id)
        {
            await _unitOfWork.TicketTypeRepository.DeleteAsync(id);
            // Optionally, call _unitOfWork.Complete() here if needed.
            return NoContent();
        }
    }
}
