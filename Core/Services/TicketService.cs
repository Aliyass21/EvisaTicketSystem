using System.Security.Claims;
using EVisaTicketSystem.Core.Data;
using EVisaTicketSystem.Core.DTOs;
using EVisaTicketSystem.Core.Entities;
using EVisaTicketSystem.Core.Enums;
using EVisaTicketSystem.Core.Interfaces;
using EVisaTicketSystem.Specifcation.Tickets;

namespace EVisaTicketSystem.Core.Services
{
    public class TicketService : ITicketService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPhotoService _photoService; // Inject your photo service
        private readonly NotificationService _notificationService; // Add this
        private readonly TicketStateMachine _ticketStateMachine; // Inject the state machine



        public TicketService(
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            IPhotoService photoService,
            NotificationService notificationService, // Inject here
            TicketStateMachine ticketStateMachine) // Inject here

        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _photoService = photoService;
            _notificationService = notificationService; // Store reference
            _ticketStateMachine = ticketStateMachine; // Store reference

        }
public async Task<(IEnumerable<Ticket> Items, int TotalCount)> SearchTicketsAsync(FilterTicketsSpecification spec)
{
    var tickets = await _unitOfWork.TicketRepository.ListAsync(spec);
    var totalCount = await _unitOfWork.TicketRepository.CountAsync(spec);
    
    return (tickets, totalCount);
}
private async Task NotifyTicketCreatorAsync(Guid ticketId, string message)
{
    var ticket = await _unitOfWork.TicketRepository.GetByIdAsync(ticketId);
    if (ticket == null) throw new Exception("Ticket not found.");

    // Build notification
    var notification = new Notification
    {
        Message = message,
        IsRead = false,
        UserId = ticket.CreatedById
    };

    // Optionally store the notification in your DB before sending
    // (Assuming you have a NotificationRepository on your UnitOfWork)
     await _unitOfWork.NotificationRepository.CreateAsync(notification);
     await _unitOfWork.Complete();

    // Then send it via SignalR (real-time push)
    await _notificationService.SendNotificationAsync(notification);
}


        // Helper method to get the current user's ID
        private Guid GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (Guid.TryParse(userIdClaim, out Guid userId))
            {
                return userId;
            }

            throw new InvalidOperationException("User ID not found or invalid.");
        }

        // Helper method to get the current user's role
private string GetUserRole()
{
    var user = _httpContextAccessor.HttpContext?.User;
    if (user == null)
        throw new InvalidOperationException("User not found in HttpContext.");
    
    // Log all claims for debugging purposes (remove in production)
    foreach (var claim in user.Claims)
    {
        Console.WriteLine($"Claim: {claim.Type} = {claim.Value}");
    }

    var roleClaim = user.FindFirst(ClaimTypes.Role)?.Value;
    if (!string.IsNullOrEmpty(roleClaim))
    {
        return roleClaim;
    }

    throw new InvalidOperationException("User role not found.");
}
public async Task ApproveTicketAsync(Guid ticketId, string notes)
{
    // Retrieve the ticket by its id
    var ticket = await _unitOfWork.TicketRepository.GetByIdAsync(ticketId);
    if (ticket == null)
    {
        throw new Exception("Ticket not found.");
    }

    // Get the current user's id (approver's id)
    var approverId = GetCurrentUserId();

    // Update the AssignedToId so that the ticket is now assigned to the approver
    ticket.AssignedToId = approverId;

    // Optionally log the approval action. You can define a new action type called Approved

    // Update the ticket via the repository and commit the changes
    await _unitOfWork.TicketRepository.UpdateAsync(ticket);
    await _unitOfWork.Complete();
}
public async Task<Ticket> UpdateTicketDetailsAsync(Guid ticketId, TicketUpdateDto updateDto)
{
    // Retrieve the ticket
    var ticket = await _unitOfWork.TicketRepository.GetByIdAsync(ticketId);
    if (ticket == null)
    {
        throw new Exception("Ticket not found.");
    }

    // Update fields only if a new value is provided
    if (!string.IsNullOrEmpty(updateDto.Title))
    {
        ticket.Title = updateDto.Title;
    }
    if (!string.IsNullOrEmpty(updateDto.Description))
    {
        ticket.Description = updateDto.Description;
    }
    if (updateDto.TicketTypeId.HasValue)
    {
        ticket.TicketTypeId = updateDto.TicketTypeId.Value;
    }
    if (updateDto.OfficeId.HasValue)
    {
        ticket.OfficeId = updateDto.OfficeId.Value;
    }
    // Process the attachment if provided
    if (updateDto.Attachment != null)
    {
        // Assuming _photoService.AddPhotoAsync returns an object with a FilePath property.
        var uploadResult = await _photoService.AddPhotoAsync(updateDto.Attachment, ticket.Id);
        // Optionally update existing attachment or add new
        // For simplicity, let's add it as a new attachment:
        var attachment = new TicketAttachment
        {
            TicketId = ticket.Id,
            FilePath = uploadResult.FilePath
        };
        ticket.Attachments.Add(attachment);
    }

    await _unitOfWork.TicketRepository.UpdateAsync(ticket);
    await _unitOfWork.Complete();

    return ticket;
}

private async Task<string> GetNextTicketNumberAsync()
{
    // Fetch the most recently created ticket.
    // You might need to implement a method like GetLastTicketAsync in your repository
    // that orders by TicketNumber or creation date.
    var lastTicket = await _unitOfWork.TicketRepository.GetLastTicketAsync();

    if (lastTicket == null || string.IsNullOrEmpty(lastTicket.TicketNumber))
    {
        // If no tickets exist, start with TCKT001
        return "TCKT001";
    }

    // Assuming the TicketNumber is in the format "TCKT" + 3-digit number
    var prefix = "TCKT";
    var numericPart = lastTicket.TicketNumber.Replace(prefix, "");

    if (int.TryParse(numericPart, out int lastNumber))
    {
        // Increment and pad with zeros (assuming you want 3-digit formatting)
        return $"{prefix}{(lastNumber + 1).ToString("D3")}";
    }
    else
    {
        // Fallback in case parsing fails.
        return $"{prefix}001";
    }
}



        // Create a new ticket using a TicketDto
         // Create a new ticket using TicketCreateDto
public async Task<Ticket> CreateTicketAsync(TicketCreateDto ticketDto)
{
    if (ticketDto.CreatedById == Guid.Empty)
    {
        ticketDto.CreatedById = GetCurrentUserId();
    }

    // Get the current user's role
    var currentUserRole = GetUserRole();

    // Generate the ticket number in the backend
    var generatedTicketNumber = await GetNextTicketNumberAsync();

    // Determine the initial status and stage based on the role of the creator
    TicketStatus initialStatus = TicketStatus.New;
    TicketStage initialStage = TicketStage.ResidenceUser;

    if (currentUserRole == "Admin")
    {
        initialStatus = TicketStatus.Resolved;
        initialStage = TicketStage.ScopeSky;
    }
    else if (currentUserRole == "SubAdmin")
    {
        initialStatus = TicketStatus.Escalated;
        initialStage = TicketStage.SystemAdmin;
    }

    // Map properties from the DTO to a new Ticket and assign the generated TicketNumber
    var ticket = new Ticket
    {
        TicketNumber = generatedTicketNumber,
        Title = ticketDto.Title,
        Description = ticketDto.Description,
        Status = initialStatus,  // Set the determined initial status
        CurrentStage = initialStage, // Set the determined initial stage
        Priority = ticketDto.Priority,
        TicketTypeId = ticketDto.TicketTypeId,
        CreatedById = ticketDto.CreatedById,
        AssignedToId = ticketDto.AssignedToId,
        OfficeId = ticketDto.OfficeId,
        ClosedAt = ticketDto.ClosedAt
    };

    // Create the ticket via repository
    await _unitOfWork.TicketRepository.CreateAsync(ticket);

    // Log the Created action
    var createdAction = new TicketAction
    {
        TicketId = ticket.Id,
        UserId = ticketDto.CreatedById,
        ActionType = TicketActionType.Created,
        ActionDate = DateTime.UtcNow,
        Notes = "Ticket created.",
        NewStatus = ticket.Status,
        NewStage = ticket.CurrentStage
    };
    ticket.Actions.Add(createdAction);

    // If an attachment file is provided, process it.
    if (ticketDto.Attachment != null)
    {
        var uploadResult = await _photoService.AddPhotoAsync(ticketDto.Attachment, ticket.Id);
        var attachment = new TicketAttachment
        {
            TicketId = ticket.Id,
            FilePath = uploadResult.FilePath
        };
        ticket.Attachments.Add(attachment);
    }

    await _unitOfWork.Complete();
    return ticket;
}
        public async Task DeleteTicketAsync(Guid id)
        {
            // Optionally check if the ticket exists first.
            var ticket = await _unitOfWork.TicketRepository.GetByIdAsync(id);
            if (ticket == null)
            {
                throw new Exception("Ticket not found.");
            }
            
            await _unitOfWork.TicketRepository.DeleteAsync(id);
            await _unitOfWork.Complete();
        }

        // Get ticket by Id
public async Task<TicketDetailDto> GetTicketByIdAsync(Guid id)
{
    var ticket = await _unitOfWork.TicketRepository.GetByIdWithDetailsAsync(id);
    
    if (ticket == null)
    {
        return null;
    }
    
    var ticketDetailDto = new TicketDetailDto
    {
        Id = ticket.Id,
        TicketNumber = ticket.TicketNumber,
        Title = ticket.Title,
        Description = ticket.Description ?? string.Empty,
        Status = ticket.Status,
        CurrentStage = ticket.CurrentStage,
        Priority = ticket.Priority,
        
        TicketTypeId = ticket.TicketTypeId,
        TicketTypeName = ticket.TicketType?.Title ?? string.Empty,
        
        CreatedById = ticket.CreatedById,
        CreatedByName = ticket.CreatedBy?.FullName ?? string.Empty,
        
        AssignedToId = ticket.AssignedToId,
        AssignedToName = ticket.AssignedTo?.FullName ?? string.Empty,
        
        OfficeId = ticket.OfficeId,
        OfficeName = ticket.Office?.Title ?? string.Empty,
        
        CreatedAt = ticket.DateCreated,
        ClosedAt = ticket.ClosedAt,
        
        Actions = ticket.Actions
        .OrderBy(a => a.ActionDate)
        .Select(a => new TicketActionDto
        {
            Id = a.Id,
            TicketId = a.TicketId,
            UserId = a.UserId,
            UserName = a.User?.FullName ?? string.Empty,
            ActionType = a.ActionType,
            ActionDate = a.ActionDate,
            Notes = a.Notes ?? string.Empty,
            PreviousStatus = a.PreviousStatus,
            NewStatus = a.NewStatus,
            PreviousStage = a.PreviousStage,
            NewStage = a.NewStage
        }).ToList()
    };
    
    return ticketDetailDto;
}

        // Get all tickets
        public async Task<IEnumerable<Ticket>> GetAllTicketsAsync()
        {
            return await _unitOfWork.TicketRepository.GetAllAsync();
        }

        // Update ticket (FSM Transition)
        
 public async Task UpdateTicketAsync(Guid ticketId, TicketActionType actionType, string notes = "")
        {
            var ticket = await _unitOfWork.TicketRepository.GetByIdAsync(ticketId);
            if (ticket == null) throw new Exception("Ticket not found.");

            var currentUserRole = GetUserRole();
            var currentStage = ticket.CurrentStage;
            var currentStatus = ticket.Status;

            // Use the state machine to determine the next state
            (TicketStatus newStatus, TicketStage newStage) = _ticketStateMachine.GetNextState(
                currentStatus, currentStage, actionType, currentUserRole);

            // Update the ticket
            ticket.Status = newStatus;
            ticket.CurrentStage = newStage;

            // Log the action
            var ticketAction = new TicketAction
            {
                TicketId = ticket.Id,
                UserId = GetCurrentUserId(),
                ActionType = actionType,
                ActionDate = DateTime.UtcNow,
                Notes = notes,
                PreviousStatus = currentStatus,
                NewStatus = newStatus,
                PreviousStage = currentStage,
                NewStage = newStage
            };

            ticket.Actions.Add(ticketAction);

            // If the ticket is closed, set ClosedAt
            if (newStatus == TicketStatus.Closed)
                ticket.ClosedAt = DateTime.UtcNow;

            // Update the ticket in the DB
            await _unitOfWork.TicketRepository.UpdateAsync(ticket);
            await _unitOfWork.Complete();

            // Notify the ticket creator
            var statusArabic = TranslateStatusToArabic(ticket.Status);
            var notificationMessage = $"تم تغيير حالة التذكرة رقم '{ticket.TicketNumber}' إلى {statusArabic}.";
            await NotifyTicketCreatorAsync(ticketId, notificationMessage);
        }

private string TranslateStatusToArabic(TicketStatus status)
{
    return status switch
    {
        TicketStatus.New        => "جديدة",
        TicketStatus.InReview   => "قيد المراجعة",
        TicketStatus.Returned   => "معادة",
        TicketStatus.Escalated  => "تم تصعيدها",
        TicketStatus.InProgress => "قيد التنفيذ",
        TicketStatus.Rejected   => "مرفوضة",
        TicketStatus.Cancelled  => "ملغاة",
        TicketStatus.Resolved   => "تم حلها",
        TicketStatus.Closed     => "مغلقة",
        _ => "غير معروفة" // fallback if needed
    };
}
    }
}
