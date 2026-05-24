using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskManagement.Application.Auth.Interfaces;
using TaskManagement.Application.Departments.Interfaces;
using TaskManagement.Application.Users.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Persistence;
using TaskManagement.Infrastructure.Repositories;
using TaskManagement.Infrastructure.Services;

namespace TaskManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(opt =>
            opt.UseNpgsql(config.GetConnectionString("DefaultConnection")));

        services.AddIdentityCore<AppUser>(opt =>
        {
            opt.Password.RequireNonAlphanumeric = false;
            opt.Password.RequireUppercase = false;
        })
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();

        return services;
    }
}
