using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Users.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskManagement.Infrastructure.Persistence;

namespace TaskManagement.Infrastructure.Repositories;

public class UserRepository(AppDbContext db) : IUserRepository
{
    public async Task<AppUser?> GetByIdAsync(string id, CancellationToken ct = default) =>
        await db.Users.Include(u => u.Department).FirstOrDefaultAsync(u => u.Id == id, ct);

    public async Task<IReadOnlyList<AppUser>> GetAllAsync(CancellationToken ct = default) =>
        await db.Users.Include(u => u.Department).ToListAsync(ct);

    public async Task UpdateAsync(AppUser user, CancellationToken ct = default)
    {
        db.Users.Update(user);
        await db.SaveChangesAsync(ct);
    }

    public async Task<List<string>> GetGrantedPermissionsAsync(string userId, CancellationToken ct = default) =>
        await db.UserPermissions
            .Where(up => up.UserId == userId && up.IsGranted)
            .Select(up => up.Permission.ToString())
            .ToListAsync(ct);

    public async Task SetPermissionsAsync(string userId, List<Permission> permissions, CancellationToken ct = default)
    {
        var existing = await db.UserPermissions
            .Where(up => up.UserId == userId)
            .ToListAsync(ct);

        db.UserPermissions.RemoveRange(existing);

        var newPermissions = permissions.Select(p => new UserPermission
        {
            UserId = userId,
            Permission = p,
            IsGranted = true
        });

        db.UserPermissions.AddRange(newPermissions);
        await db.SaveChangesAsync(ct);
    }
}
