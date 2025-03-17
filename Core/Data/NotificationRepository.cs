using EVisaTicketSystem.Core.Data;
using EVisaTicketSystem.Core.Entities;
using EVisaTicketSystem.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EVisaTicketSystem.Data.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly DataContext _context;

        public NotificationRepository(DataContext context)
        {
            _context = context;
        }
            public async Task<IEnumerable<Notification>> GetNotificationsForUserAsync(Guid userId)
    {
        // Return IEnumerable instead of List
        return await _context.Notifications
            .Where(n => n.UserId == userId || n.UserId == Guid.Empty)
            .OrderByDescending(n => n.DateCreated)
            .ToListAsync();
    }

        public async Task<Notification> CreateAsync(Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
            return notification;
        }

       public async Task<IEnumerable<Notification>> GetAllAsync()
{
    return await _context.Notifications
        .OrderByDescending(n => n.DateCreated)
        .ToListAsync();
}


        public async Task<Notification> GetByIdAsync(Guid id)
        {
            return await _context.Notifications.FindAsync(id);
        }
              public void Add(Notification notification)
        {
            _context.Notifications.Add(notification);
        }
        public async Task UpdateAsync(Notification notification)
{
    _context.Notifications.Update(notification);
    await _context.SaveChangesAsync();
}


        // New method: Get notifications for a specific user.
     // Add this to your NotificationRepository class

    }
}
