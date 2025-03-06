using EVisaTicketSystem.Core.Data;
using EVisaTicketSystem.Core.DTOs;
using EVisaTicketSystem.Core.Enums;
using EVisaTicketSystem.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EVisaTicketSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        // GET: api/Ticket
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ticket>>> GetTickets()
        {
            var tickets = await _ticketService.GetAllTicketsAsync();
            return Ok(tickets);
        }

        // GET: api/Ticket/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Ticket>> GetTicket(Guid id)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(id);
            return ticket == null ? NotFound() : Ok(ticket);
        }

        // POST: api/Ticket (Create by ResidenceUser)
        [HttpPost]
        [Authorize(Roles = "ResidenceUser")]
        public async Task<ActionResult<Ticket>> CreateTicket([FromBody] TicketDto ticketDto)
        {
            var ticket = await _ticketService.CreateTicketAsync(ticketDto);
            return CreatedAtAction(nameof(GetTicket), new { id = ticket.Id }, ticket);
        }

        // POST: api/Ticket/{id}/submit (ResidenceUser)
        [HttpPost("{id}/submit")]
        [Authorize(Roles = "ResidenceUser")]
        public async Task<IActionResult> SubmitTicket(Guid id, [FromBody] string notes)
        {
            await _ticketService.UpdateTicketAsync(id, TicketActionType.StatusChanged, notes);
            return NoContent();
        }

        // POST: api/Ticket/{id}/approve (SubAdmin)
        [HttpPost("{id}/approve")]
        [Authorize(Roles = "SubAdmin")]
        public async Task<IActionResult> ApproveTicket(Guid id, [FromBody] string notes)
        {
            await _ticketService.UpdateTicketAsync(id, TicketActionType.StatusChanged, notes);
            return NoContent();
        }

        // POST: api/Ticket/{id}/return (SubAdmin)
        [HttpPost("{id}/return")]
        [Authorize(Roles = "SubAdmin")]
        public async Task<IActionResult> ReturnTicket(Guid id, [FromBody] string notes)
        {
            await _ticketService.UpdateTicketAsync(id, TicketActionType.Returned, notes);
            return NoContent();
        }

        // POST: api/Ticket/{id}/escalate (SubAdmin)
        [HttpPost("{id}/escalate")]
        [Authorize(Roles = "SubAdmin")]
        public async Task<IActionResult> EscalateTicket(Guid id, [FromBody] string notes)
        {
            await _ticketService.UpdateTicketAsync(id, TicketActionType.Escalated, notes);
            return NoContent();
        }

        // POST: api/Ticket/{id}/resolve (SystemAdmin)
        [HttpPost("{id}/resolve")]
        [Authorize(Roles = "SystemAdmin")]
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