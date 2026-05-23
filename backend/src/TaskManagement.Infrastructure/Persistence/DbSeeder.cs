using Microsoft.AspNetCore.Identity;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAdminAsync(UserManager<AppUser> userManager)
    {
        const string email = "himalayan@gmail.com";
        const string password = "Himalayan@123";

        if (await userManager.FindByEmailAsync(email) is null)
        {
            var admin = new AppUser { UserName = email, Email = email, EmailConfirmed = true };
            await userManager.CreateAsync(admin, password);
        }
    }
}
