using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Users.Interfaces;

public interface IUserRepository
{
    Task<AppUser?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<IReadOnlyList<AppUser>> GetAllAsync(CancellationToken ct = default);
    Task UpdateAsync(AppUser user, CancellationToken ct = default);
}
