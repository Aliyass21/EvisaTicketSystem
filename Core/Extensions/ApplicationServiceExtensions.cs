using System;
using EVisaTicketSystem.Core.Data;
using EVisaTicketSystem.Core.Helpers;
using EVisaTicketSystem.Core.Interfaces;
using EVisaTicketSystem.Core.Services;
using EVisaTicketSystem.Data.Repositories;
using EVisaTicketSystem.Infrastructure.Repositories;
using EVisaTicketSystem.Infrastructure.Services;
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
       services.AddScoped<ITicketRepository, TicketRepository>();
       services.AddScoped<ITicketTypeRepository, TicketTypeRepository>(); // Add this line
       services.AddScoped<ITicketAttachmentRepository, TicketAttachmentRepository>();
       services.AddScoped<ITicketService, TicketService>();
       services.AddScoped<IPhotoService, PhotoService>();
       services.AddScoped<IOfficeRepository, OfficeRepository>(); // Add this line
       services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
       services.AddScoped<LogUserActivity>();
       services.AddScoped<IUnitOfWork, UnitOfWork>();
       services.AddSignalR();
       services.AddScoped<TicketStateMachine>(); // Register TicketStateMachine
       //services.AddSingleton<NotificationService>();
       services.AddScoped<NotificationService>();  // <-- changed from .AddSingleton()

       services.AddScoped<INotificationRepository, NotificationRepository>();




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

