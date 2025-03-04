using System;
using EVisaTicketSystem.Core.Data;

namespace EVisaTicketSystem.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
       services.AddControllers();
    //    services.AddDbContext<DataContext>(opt =>
    //     {
    //         opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
    //     });

    //    services.AddCors();
    //    services.AddScoped<ITokenService, TokenService>();
    //    services.AddScoped<IUserRepository, UserRepository>();
    //    services.AddScoped<IPhotoService,PhotoService>();
    //    services.AddScoped<ILikeRepository,LikesRepository>();
    //    services.AddScoped<IMessageRepository,MessageRespository>();
    //    services.AddScoped<IUnitOfWork, UnitOfWork>();
    //    services.AddScoped<LogUserActivity>();
    //    services.AddSignalR();
    //    services.AddSingleton<PresenceTracker>();
    //    services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
    //    services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));


       return services;
    }
}

