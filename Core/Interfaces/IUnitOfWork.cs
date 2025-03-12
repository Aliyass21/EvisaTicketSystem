using System;
using EVisaTicketSystem.Interfaces;

namespace EVisaTicketSystem.Core.Interfaces;

public interface IUnitOfWork
{
    IUserRepository UserRepository {get;}
    ITicketRepository TicketRepository { get; } // Add TicketRepository
    ITicketTypeRepository TicketTypeRepository { get; } // Add this line
    IOfficeRepository OfficeRepository { get; } // Add this line
    INotificationRepository NotificationRepository { get; } // Add this line



    // IMessageRepository MessageRepository {get;}
    // ILikeRepository LikesRepository {get;}
    Task<bool> Complete();
    bool HasChanges();
}
