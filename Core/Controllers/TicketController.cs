using AutoMapper;
using EVisaTicketSystem.Core.Controllers;
using EVisaTicketSystem.Core.Data;
using EVisaTicketSystem.Core.DTOs;
using EVisaTicketSystem.Core.Enums;
using EVisaTicketSystem.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EVisaTicketSystem.API.Controllers
{

    public class TicketController : BaseApiController
    {
        private readonly ITicketService _ticketService;
        private readonly IMapper _mapper;


        public TicketController(ITicketService ticketService,IMapper mapper)
        {
            _ticketService = ticketService;
            _mapper = mapper;

        }

        // GET: api/Ticket
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<TicketResponseDto>>> GetTickets()
        {
            var tickets = await _ticketService.GetAllTicketsAsync();
            var ticketDtos = _mapper.Map<IEnumerable<TicketResponseDto>>(tickets);
            return Ok(ticketDtos);
        }

        // GET: api/Ticket/{id}
            [HttpGet("{id}")]
            [Authorize]
            public async Task<ActionResult<TicketResponseDto>> GetTicket(Guid id)
            {
                var ticket = await _ticketService.GetTicketByIdAsync(id);
                if (ticket == null)
                {
                    return NotFound();
                }
                
                var ticketDto = _mapper.Map<TicketResponseDto>(ticket);
                return Ok(ticketDto);
            }

        // POST: api/Ticket (Create by ResidenceUser)
        [HttpPost]
        [Authorize(Policy = "RequireResidenceUser")]
        public async Task<IActionResult> CreateTicket([FromForm] TicketCreateDto ticketDto)
        {
            var createdTicket = await _ticketService.CreateTicketAsync(ticketDto);

            // Return a JSON response with the new ticket ID
            return Ok(new 
            {
                message = "Ticket created successfully",
                ticketId = createdTicket.Id
            });
        }

        //PUT:api/Ticket/{guid}
        [HttpPut("{id}")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> UpdateTicketDetails(Guid id, [FromForm] TicketUpdateDto updateDto)
        {
            try
            {
                var updatedTicket = await _ticketService.UpdateTicketDetailsAsync(id, updateDto);
                return Ok(new { message = "Ticket updated successfully", ticketId = updatedTicket.Id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }



        // POST: api/Ticket/{id}/submit (ResidenceUser)
        [HttpPost("{id}/inreview")]
        [Authorize(Policy ="RequireResidenceUser")]

        //[Authorize(Roles = "ResidenceUser")]
        public async Task<IActionResult> SubmitTicket(Guid id, [FromBody] string notes)
        {
            await _ticketService.UpdateTicketAsync(id, TicketActionType.StatusChanged, notes);
            return NoContent();
        }
        // POST: api/Ticket/{id}/inprogress (Mark as InProgress by SystemAdmin)
        [HttpPost("{id}/inprogress")]
        [Authorize(Policy ="RequireAdminRole")]

        public async Task<IActionResult> MarkTicketInProgress(Guid id, [FromBody] string notes)
        {
            // From Escalated state, use InProgress action.
            await _ticketService.UpdateTicketAsync(id, TicketActionType.InProgress, notes);
            return NoContent();
        }

        // POST: api/Ticket/{id}/reject (Reject ticket by SystemAdmin)
        [HttpPost("{id}/reject")]
        [Authorize(Policy ="RequireAdminRole")]

        public async Task<IActionResult> RejectTicket(Guid id, [FromBody] string notes)
        {
            // From Escalated or InProgress state, use Rejected action.
            await _ticketService.UpdateTicketAsync(id, TicketActionType.Rejected, notes);
            return NoContent();
        }

        // POST: api/Ticket/{id}/cancel (Cancel ticket by SystemAdmin)
        [HttpPost("{id}/cancel")]
        [Authorize(Policy ="RequireAdminRole")]

        public async Task<IActionResult> CancelTicket(Guid id, [FromBody] string notes)
        {
            // From InProgress state, use Cancelled action.
            await _ticketService.UpdateTicketAsync(id, TicketActionType.Cancelled, notes);
            return NoContent();
        }
        // POST: api/Ticket/{id}/approve
[HttpPost("{id}/approve")]
[Authorize]
public async Task<IActionResult> ApproveTicket(Guid id, [FromBody] string notes = "")
{
    try
    {
        await _ticketService.ApproveTicketAsync(id, notes);
        return NoContent();
    }
    catch (Exception ex)
    {
        return BadRequest(new { message = ex.Message });
    }
}


        // // POST: api/Ticket/{id}/approve (SubAdmin)
        // [HttpPost("{id}/approve")]
        // //[Authorize(Roles = "SubAdmin")]
        // public async Task<IActionResult> ApproveTicket(Guid id, [FromBody] string notes)
        // {
        //     await _ticketService.UpdateTicketAsync(id, TicketActionType.StatusChanged, notes);
        //     return NoContent();
        // }

        // POST: api/Ticket/{id}/return (SubAdmin)
        [HttpPost("{id}/return")]
        [Authorize(Roles = "SubAdminRole")]
        public async Task<IActionResult> ReturnTicket(Guid id, [FromBody] string notes)
        {
            await _ticketService.UpdateTicketAsync(id, TicketActionType.Returned, notes);
            return NoContent();
        }

        // POST: api/Ticket/{id}/escalate (SubAdmin)
        [HttpPost("{id}/escalate")]
        [Authorize(Roles = "SubAdminRole")]
        public async Task<IActionResult> EscalateTicket(Guid id, [FromBody] string notes)
        {
            await _ticketService.UpdateTicketAsync(id, TicketActionType.Escalated, notes);
            return NoContent();
        }

        // POST: api/Ticket/{id}/resolve (SystemAdmin)
        [HttpPost("{id}/resolve")]
        [Authorize(Roles = "RequireAdminRole")]
        public async Task<IActionResult> ResolveTicket(Guid id, [FromBody] string notes)
        {
            await _ticketService.UpdateTicketAsync(id, TicketActionType.Resolved, notes);
            return NoContent();
        }

        // POST: api/Ticket/{id}/close (ScopeSky)
        [HttpPost("{id}/close")]
        [Authorize(Roles = "ScopeSky")]
        public async Task<IActionResult> CloseTicket(Guid id, [FromBody] string notes)
        {
            await _ticketService.UpdateTicketAsync(id, TicketActionType.Closed, notes);
            return NoContent();
        }
        // DELETE: api/Ticket/{id}
        [HttpDelete("{id}")]
        [Authorize(Policy = "SubAdminRole")] // Adjust the role or policy as needed
        public async Task<IActionResult> DeleteTicket(Guid id)
        {
            try
            {
                await _ticketService.DeleteTicketAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                // Optionally log the error and return an appropriate error response
                return NotFound(new { message = ex.Message });
            }
        }

    }
}