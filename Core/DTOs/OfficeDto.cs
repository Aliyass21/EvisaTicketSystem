using EVisaTicketSystem.Core.Enums;

namespace EVisaTicketSystem.Core.DTOs
{
    public class OfficeDto
    {
        public required string Title { get; set; }
        public required OfficeType OfficeType { get; set; }
    }
}
