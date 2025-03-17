using AutoMapper;
using EVisaTicketSystem.Core.Controllers;
using EVisaTicketSystem.Core.Data;
using EVisaTicketSystem.Core.DTOs;
using EVisaTicketSystem.Core.Enums;
using EVisaTicketSystem.Core.Interfaces;
using EVisaTicketSystem.Specifcation.Tickets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EVisaTicketSystem.API.Controllers
{
    public class TicketController : BaseApiController
    {
        private readonly ITicketService _ticketService;
        private readonly IMapper _mapper;

        public TicketController(ITicketService ticketService, IMapper mapper)
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
        public async Task<ActionResult<TicketDetailDto>> GetTicket(Guid id)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }
            
            var ticketDto = _mapper.Map<TicketDetailDto>(ticket);
            return Ok(ticketDto);
        }
            
        [HttpPost("search")]
        [Authorize]
        public async Task<IActionResult> SearchTickets([FromBody] TicketSearchParams searchParams)
        {
            var spec = new FilterTicketsSpecification(
                ticketNumber: searchParams.TicketNumber,
                title: searchParams.Title,
                officeId: searchParams.OfficeId,
                status: searchParams.Status,
                startDate: searchParams.StartDate,
                endDate: searchParams.EndDate,
                skip: (searchParams.PageNumber - 1) * searchParams.PageSize,
                take: searchParams.PageSize,
                sortBy: searchParams.SortBy,
                isDescending: searchParams.IsDescending
            );

            var result = await _ticketService.SearchTicketsAsync(spec);
            var ticketDtos = _mapper.Map<IEnumerable<TicketResponseDto>>(result.Items);

            return Ok(new
            {
                Items = ticketDtos,
                TotalCount = result.TotalCount,
                PageSize = searchParams.PageSize,
                PageNumber = searchParams.PageNumber
            });
        }

        // POST: api/Ticket (Create by ResidenceUser or SubAdmin)
        [HttpPost]
        [Authorize] // Policy should check for ResidenceUser or SubAdmin
        public async Task<IActionResult> CreateTicket([FromForm] TicketCreateDto ticketDto)
        {
            var createdTicket = await _ticketService.CreateTicketAsync(ticketDto);

            return Ok(new 
            {
                message = "Ticket created successfully",
                ticketId = createdTicket.Id
            });
        }

        // PUT: api/Ticket/{guid}
        [HttpPut("{id}")]
        [Authorize]
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

        // POST: api/Ticket/{id}/inreview (ResidenceUser submits New or Returned ticket)
        [HttpPost("{id}/inreview")]
        [Authorize(Policy = "RequireResidenceUser")]
        public async Task<IActionResult> SubmitTicket(Guid id, [FromBody] string notes)
        {
            await _ticketService.UpdateTicketAsync(id, TicketActionType.StatusChanged, notes);
            return NoContent();
        }

        // POST: api/Ticket/{id}/approve (SubAdmin approves ticket)
        [HttpPost("{id}/approve")]
        [Authorize(Policy = "SubAdminRole")]
        public async Task<IActionResult> ApproveTicket(Guid id, [FromBody] ApproveTicketRequest request = null)
        {
            var notes = request?.Notes ?? "";
            
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

        // POST: api/Ticket/{id}/return (SubAdmin returns ticket to ResidenceUser)
        [HttpPost("{id}/return")]
        [Authorize(Policy = "SubAdminRole")]
        public async Task<IActionResult> ReturnTicket(Guid id, [FromBody] string notes)
        {
            await _ticketService.UpdateTicketAsync(id, TicketActionType.Returned, notes);
            return NoContent();
        }

        // POST: api/Ticket/{id}/escalate (SubAdmin escalates ticket to SystemAdmin)
        [HttpPost("{id}/escalate")]
        [Authorize(Policy = "SubAdminRole")]
        public async Task<IActionResult> EscalateTicket(Guid id, [FromBody] string notes)
        {
            await _ticketService.UpdateTicketAsync(id, TicketActionType.Escalated, notes);
            return NoContent();
        }

        // POST: api/Ticket/{id}/inprogress (Mark as InProgress by SystemAdmin or ScopeSky)
        [HttpPost("{id}/inprogress")]
        [Authorize]
        public async Task<IActionResult> MarkTicketInProgress(Guid id, [FromBody] string notes)
        {
            await _ticketService.UpdateTicketAsync(id, TicketActionType.InProgress, notes);
            return NoContent();
        }

        // POST: api/Ticket/{id}/reject (Reject ticket by SystemAdmin or ScopeSky)
        [HttpPost("{id}/reject")]
        [Authorize]
        public async Task<IActionResult> RejectTicket(Guid id, [FromBody] string notes)
        {
            await _ticketService.UpdateTicketAsync(id, TicketActionType.Rejected, notes);
            return NoContent();
        }

        // POST: api/Ticket/{id}/cancel (Cancel ticket by SystemAdmin or ScopeSky)
        [HttpPost("{id}/cancel")]
        [Authorize]
        public async Task<IActionResult> CancelTicket(Guid id, [FromBody] string notes)
        {
            await _ticketService.UpdateTicketAsync(id, TicketActionType.Cancelled, notes);
            return NoContent();
        }

        // POST: api/Ticket/{id}/senttoscopesky (Admin sends ticket to ScopeSky)
        [HttpPost("{id}/senttoscopesky")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> SendToScopesky(Guid id, [FromBody] string notes)
        {
            await _ticketService.UpdateTicketAsync(id, TicketActionType.Resolved, notes);
            return NoContent();
        }

        // POST: api/Ticket/{id}/resolve (ScopeSky resolves ticket)
        [HttpPost("{id}/resolve")]
        [Authorize(Policy = "ScopeSky")]
        public async Task<IActionResult> ResolveTicket(Guid id, [FromBody] string notes)
        {
            await _ticketService.UpdateTicketAsync(id, TicketActionType.Resolved, notes);
            return NoContent();
        }

        // POST: api/Ticket/{id}/review (Admin reviews resolved ticket)
        [HttpPost("{id}/review")]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> ReviewTicket(Guid id, [FromBody] string notes)
        {
            await _ticketService.UpdateTicketAsync(id, TicketActionType.StatusChanged, notes);
            return NoContent();
        }

        // POST: api/Ticket/{id}/close (ScopeSky closes ticket)
        [HttpPost("{id}/close")]
        [Authorize(Policy = "ScopeSky")]
        public async Task<IActionResult> CloseTicket(Guid id, [FromBody] string notes)
        {
            await _ticketService.UpdateTicketAsync(id, TicketActionType.Closed, notes);
            return NoContent();
        }

        // DELETE: api/Ticket/{id}
        [HttpDelete("{id}")]
        [Authorize(Policy = "SubAdminRole")]
        public async Task<IActionResult> DeleteTicket(Guid id)
        {
            try
            {
                await _ticketService.DeleteTicketAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}