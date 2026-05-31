using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskManagement.Infrastructure.Persistence;

namespace TaskManagement.Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, AppDbContext db)
    {
        // Seed roles
        foreach (var role in Enum.GetNames<UserRole>())
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        // Seed departments
        if (!await db.Departments.AnyAsync())
        {
            db.Departments.AddRange(
                new Department { Name = "Software Developer" },
                new Department { Name = "Finance" },
                new Department { Name = "Operations" },
                new Department { Name = "HR" }
            );
            await db.SaveChangesAsync();
        }

        // Seed default admin
        const string email = "himalayan@gmail.com";
        const string password = "Himalayan@123";

        if (await userManager.FindByEmailAsync(email) is null)
        {
            var admin = new AppUser
            {
                UserName = email,
                Email = email,
                FullName = "System Admin",
                Role = UserRole.Admin,
                EmailConfirmed = true
            };

            await userManager.CreateAsync(admin, password);
            await userManager.AddToRoleAsync(admin, UserRole.Admin.ToString());
        }

        if (!await db.Statuses.AnyAsync())
        {
            db.Statuses.AddRange(
                new Status { Name = "Open" },
                new Status { Name = "In Progress" },
                new Status { Name = "Completed" },
                new Status { Name = "On Hold" },
                new Status { Name = "Closed" }
            );

            await db.SaveChangesAsync();
        }

        if(!await db.Priorities.AnyAsync())
        {
            db.Priorities.AddRange(
                new Priority { Name = "High" },
                new Priority { Name = "Medium" },
                new Priority { Name = "Low" }
                );
            await db.SaveChangesAsync();
        }

        if (!await db.SprintStatuses.AnyAsync())
        {
            db.SprintStatuses.AddRange(
                new SprintStatusTrigger { Name = "In Progress" },
                new SprintStatusTrigger { Name = "Closed", IsActive = false }
                );  
            await db.SaveChangesAsync();
        }

    }
}
