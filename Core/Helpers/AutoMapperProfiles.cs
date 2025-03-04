using System;
using AutoMapper;
using EVisaTicketSystem.Core.DTOs;
using EVisaTicketSystem.Core.Entities;

namespace EVisaTicketSystem.Core.Helpers;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {


        CreateMap<RegisterDto,AppUser>();
        CreateMap<string, DateOnly>().ConvertUsing(s => DateOnly.Parse(s));
        CreateMap<DateTime, DateTime>().ConvertUsing(d => DateTime.SpecifyKind(d, DateTimeKind.Utc));
        CreateMap<DateTime?, DateTime?>().ConvertUsing(d => d.HasValue 
            ? DateTime.SpecifyKind(d.Value, DateTimeKind.Utc) : null);
    }

}
