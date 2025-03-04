using System;
using EVisaTicketSystem.Core.Data;
using EVisaTicketSystem.Interfaces;
using EVisaTicketSystem.Services;

namespace EVisaTicketSystem.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
       services.AddControllers();
       services.AddScoped<ITokenService, TokenService>();
       services.AddScoped<IUserRepository, UserRepository>();
       services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


    //    services.AddDbContext<DataContext>(opt =>
    //     {
    //         opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
    //     });

    //    services.AddCors();
    //    services.AddScoped<IPhotoService,PhotoService>();
    //    services.AddScoped<IUnitOfWork, UnitOfWork>();



       return services;
    }
}

