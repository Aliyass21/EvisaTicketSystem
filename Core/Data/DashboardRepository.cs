using EVisaTicketSystem.Core.Data;
using EVisaTicketSystem.Core.DTOs;
using EVisaTicketSystem.Core.Enums;
using EVisaTicketSystem.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EVisaTicketSystem.Infrastructure.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly DataContext _context;

        public DashboardRepository(DataContext context)
        {
            _context = context;
        }

public async Task<DashboardSummaryDto> GetDashboardSummaryAsync()
{
    // Total number of tickets at all time
    var totalTickets = await _context.Tickets.CountAsync();

    // Closed tickets this month.
    var now = DateTime.UtcNow;
    // Ensure first day of month is set to UTC
    var firstDayOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);

    var closedTicketsThisMonth = await _context.Tickets
        .CountAsync(t => t.Status == TicketStatus.Closed && t.DateCreated >= firstDayOfMonth);

    // Total number of users in the system
    var totalUsers = await _context.Users.CountAsync();

    return new DashboardSummaryDto
    {
        TotalTickets = totalTickets,
        ClosedTicketsThisMonth = closedTicketsThisMonth,
        TotalUsers = totalUsers
    };
}

        public async Task<IEnumerable<DailyTicketSummaryDto>> GetDailyTicketSummaryForCurrentMonthFromTodayAsync()
        {
            // Use UTC for consistency.
            var now = DateTime.UtcNow;
            var startDate = now.Date; // Today (UTC)
            // Get the last day of the current month.
            var daysInMonth = DateTime.DaysInMonth(now.Year, now.Month);
            var endDate = new DateTime(now.Year, now.Month, daysInMonth, 23, 59, 59, DateTimeKind.Utc);

            // Query tickets created between today and the end of the month,
            // grouping by the date part only.
            var groupedTickets = await _context.Tickets
                .Where(t => t.DateCreated >= startDate && t.DateCreated <= endDate)
                .GroupBy(t => t.DateCreated.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToListAsync();

            // Prepare the result list: iterate through each day from today until the end of the month.
            var result = new List<DailyTicketSummaryDto>();
            for (var date = startDate; date <= endDate.Date; date = date.AddDays(1))
            {
                var group = groupedTickets.FirstOrDefault(g => g.Date == date);
                result.Add(new DailyTicketSummaryDto
                {
                    Date = date,
                    TicketCount = group?.Count ?? 0
                });
            }

            return result;
        }

    }
}
