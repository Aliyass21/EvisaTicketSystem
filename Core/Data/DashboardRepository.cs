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
    var todayUtc = DateTime.UtcNow.Date;
    var startDate = new DateTime(todayUtc.Year, todayUtc.Month, 1, 0, 0, 0, DateTimeKind.Utc);
    var endDate = new DateTime(todayUtc.Year, todayUtc.Month, todayUtc.Day, 23, 59, 59, DateTimeKind.Utc);

    var groupedTickets = await _context.Tickets
        .Where(t => t.DateCreated >= startDate && t.DateCreated <= endDate)
        .GroupBy(t => t.DateCreated.Date)
        .Select(g => new { Date = g.Key, Count = g.Count() })
        .ToListAsync();

    var result = new List<DailyTicketSummaryDto>();
    for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
    {
        var match = groupedTickets.FirstOrDefault(g => g.Date == date);
        result.Add(new DailyTicketSummaryDto
        {
            Date = DateTime.SpecifyKind(date, DateTimeKind.Utc),
            TicketCount = match?.Count ?? 0
        });
    }

    return result;
}



    }
}
