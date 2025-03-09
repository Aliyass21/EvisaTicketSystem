using EVisaTicketSystem.Core.Controllers;
using EVisaTicketSystem.Core.DTOs;
using EVisaTicketSystem.Core.Entities;
using EVisaTicketSystem.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EVisaTicketSystem.API.Controllers
{

    public class TicketAttachmentController : BaseApiController
    {
        private readonly ITicketAttachmentRepository _attachmentRepository;
        private readonly IPhotoService _photoService;
        private readonly IUnitOfWork _unitOfWork; // If you're using Unit of Work

        public TicketAttachmentController(ITicketAttachmentRepository attachmentRepository, IUnitOfWork unitOfWork,IPhotoService photoService)
        {
            _attachmentRepository = attachmentRepository;
            _unitOfWork = unitOfWork;
            _photoService=photoService;
        }

        // POST: api/TicketAttachment
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<TicketAttachment>> CreateAttachment([FromBody] TicketAttachmentDto attachmentDto)
        {
            var attachment = new TicketAttachment
            {
                FilePath = attachmentDto.FilePath,
                TicketId = attachmentDto.TicketId
            };

            await _attachmentRepository.CreateAsync(attachment);
            await _unitOfWork.Complete(); // Save changes

            return CreatedAtAction(nameof(GetAttachment), new { id = attachment.Id }, attachment);
        }

        // GET: api/TicketAttachment/{id}
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<TicketAttachment>> GetAttachment(Guid id)
        {
            var attachment = await _attachmentRepository.GetByIdAsync(id);
            if (attachment == null)
                return NotFound();

            return Ok(attachment);
        }

        // GET: api/TicketAttachment/ticket/{ticketId}
        [HttpGet("ticket/{ticketId}")]
        [Authorize]
        public async Task<ActionResult> GetAttachmentsByTicketId(Guid ticketId)
        {
            var attachments = await _attachmentRepository.GetByTicketIdAsync(ticketId);
            return Ok(attachments);
        }

        // DELETE: api/TicketAttachment/{id}
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteAttachment(Guid id)
        {
            // Retrieve the TicketAttachment entity
            var attachment = await _attachmentRepository.GetByIdAsync(id);
            if (attachment == null)
            {
                return NotFound();
            }

            // Delete the physical file using the PhotoService
            bool fileDeleted = await _photoService.DeletePhotoAsync(attachment.FilePath);

            // Remove the entity from the repository
            _attachmentRepository.Delete(attachment);
            await _unitOfWork.Complete();

            // Optionally, return a status based on file deletion success.
            return fileDeleted ? NoContent() : StatusCode(500, "Attachment deleted from database but file deletion failed.");
        }
    }
}
