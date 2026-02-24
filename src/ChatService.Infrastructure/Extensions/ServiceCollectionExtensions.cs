using ChatService.Application.Interfaces;
using ChatService.Domain.Interfaces;
using ChatService.Infrastructure.Persistence;
using ChatService.Infrastructure.Persistence.Repositories;
using ChatService.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChatService.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(opts =>
            opts.UseSqlServer(config.GetConnectionString("DefaultConnection")!,
                sql => sql.EnableRetryOnFailure(maxRetryCount: 3)));

        // BUG FIX: all registered as Scoped (not Singleton) to match DbContext lifetime
        services.AddScoped<IUserRepository,    UserRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<IGroupRepository,   GroupRepository>();
        services.AddScoped<IAuthService,       AuthService>();
        services.AddScoped<IChatService,       ChatServiceImpl>();

        return services;
    }
}
