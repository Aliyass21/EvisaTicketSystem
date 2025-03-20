using EVisaTicketSystem.Core.DTOs;

namespace EVisaTicketSystem.Interfaces
{
    public interface IDashboardRepository
    {
        Task<DashboardSummaryDto> GetDashboardSummaryAsync();
        Task<IEnumerable<DailyTicketSummaryDto>> GetDailyTicketSummaryForCurrentMonthFromTodayAsync();
    }
}
