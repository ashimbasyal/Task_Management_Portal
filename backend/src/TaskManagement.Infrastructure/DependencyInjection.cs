using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskManagement.Application.Auth.Interfaces;
using TaskManagement.Application.Common.Behaviours;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Persistence;
using TaskManagement.Infrastructure.Services;

namespace TaskManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>((sp, opt) =>
            opt.UseNpgsql(config.GetConnectionString("DefaultConnection"))
               .AddInterceptors(sp.GetRequiredService<AuditingInterceptor>()));

        services.AddScoped<IApplicationDbContext>(provider =>
             provider.GetRequiredService<AppDbContext>());

        services.AddIdentityCore<AppUser>(opt =>
        {
            opt.Password.RequireNonAlphanumeric = false;
            opt.Password.RequireUppercase = false;
        })
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        services.AddScoped<AuditingInterceptor>();
        services.AddScoped<ITokenService, TokenService>();

        return services;
    }
}
