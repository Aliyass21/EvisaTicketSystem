using EVisaTicketSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EVisaTicketSystem.Core.Interfaces
{
    public interface INotificationRepository
    {
        Task<Notification> CreateAsync(Notification notification);
        void Add(Notification notification);

        Task<IEnumerable<Notification>> GetAllAsync();
        Task<Notification> GetByIdAsync(Guid id);
    Task<IEnumerable<Notification>> GetNotificationsForUserAsync(Guid userId);
    }

}
