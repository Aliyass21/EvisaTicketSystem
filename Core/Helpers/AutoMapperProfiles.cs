using AutoMapper;
using EVisaTicketSystem.Core.Data;
using EVisaTicketSystem.Core.DTOs;
using EVisaTicketSystem.Core.Entities;

namespace EVisaTicketSystem.Core.Helpers;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {

        CreateMap<AppUser, UserSummaryDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName));

        CreateMap<Ticket, TicketResponseDto>()
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy));
        CreateMap<RegisterDto,AppUser>();
        CreateMap<TicketDto,Ticket>();
        CreateMap<Ticket, TicketDto>();
        CreateMap<Ticket, TicketDetailDto>()
            .ForMember(dest => dest.TicketTypeName, opt => opt.MapFrom(src => src.TicketType != null ? src.TicketType.Title : string.Empty))
            .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => src.CreatedBy != null ? src.CreatedBy.FullName : string.Empty))
            .ForMember(dest => dest.OfficeName, opt => opt.MapFrom(src => src.Office != null ? src.Office.Title : string.Empty));
        CreateMap<TicketTypeDto,TicketType>();
        CreateMap<TicketAttachmentDto,TicketAttachment>();
        CreateMap<OfficeDto,Office>();
        CreateMap<string, DateOnly>().ConvertUsing(s => DateOnly.Parse(s));
        CreateMap<DateTime, DateTime>().ConvertUsing(d => DateTime.SpecifyKind(d, DateTimeKind.Utc));
        CreateMap<DateTime?, DateTime?>().ConvertUsing(d => d.HasValue 
            ? DateTime.SpecifyKind(d.Value, DateTimeKind.Utc) : null);
    }

}
