using EVisaTicketSystem.Core.Controllers;
using EVisaTicketSystem.Core.DTOs;
using EVisaTicketSystem.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EVisaTicketSystem.API.Controllers
{
    public class DashboardController : BaseApiController
    {
        private readonly IDashboardRepository _dashboardRepository;

        public DashboardController(IDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        // GET: api/Dashboard/dashboard-summary
        [HttpGet("dashboard-summary")]
        [Authorize]
        public async Task<ActionResult<DashboardSummaryDto>> GetDashboardSummary()
        {
            var summary = await _dashboardRepository.GetDashboardSummaryAsync();
            return Ok(summary);
        }
                // GET: api/Dashboard/daily-summary-current
        [HttpGet("daily-summary-current")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<DailyTicketSummaryDto>>> GetDailyTicketSummaryForCurrentMonthFromToday()
        {
            var dailySummary = await _dashboardRepository.GetDailyTicketSummaryForCurrentMonthFromTodayAsync();
            return Ok(dailySummary);
        }
    }
}
