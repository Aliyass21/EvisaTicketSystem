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

        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        // GET: api/Ticket
        [HttpGet]
        [Authorize]

        
        public async Task<ActionResult<IEnumerable<Ticket>>> GetTickets()
        {
            var tickets = await _ticketService.GetAllTicketsAsync();
            return Ok(tickets);
        }

        // GET: api/Ticket/{id}
        [HttpGet("{id}")]
        [Authorize]

        public async Task<ActionResult<Ticket>> GetTicket(Guid id)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(id);
            return ticket == null ? NotFound() : Ok(ticket);
        }

        // POST: api/Ticket (Create by ResidenceUser)
        [HttpPost]
        [Authorize(Policy ="SubAdminRole")]
        //[Authorize(Roles = "ResidenceUser")]
        public async Task<ActionResult<Ticket>> CreateTicket([FromForm] TicketCreateDto ticketDto)
        {
            var ticket = await _ticketService.CreateTicketAsync(ticketDto);
            return CreatedAtAction(nameof(GetTicket), new { id = ticket.Id }, ticket);
        }

        // POST: api/Ticket/{id}/submit (ResidenceUser)
        [HttpPost("{id}/inreview")]
        [Authorize(Policy ="SubAdminRole")]

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
    }
}