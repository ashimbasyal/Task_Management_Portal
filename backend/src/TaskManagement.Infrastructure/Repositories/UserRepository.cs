using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Users.Interfaces;
using TaskManagement.Domain.Entities;
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
}
