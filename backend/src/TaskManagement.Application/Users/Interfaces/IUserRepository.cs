using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.Users.Interfaces;

public interface IUserRepository
{
    Task<AppUser?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<IReadOnlyList<AppUser>> GetAllAsync(CancellationToken ct = default);
    Task UpdateAsync(AppUser user, CancellationToken ct = default);
    Task<List<string>> GetGrantedPermissionsAsync(string userId, CancellationToken ct = default);
    Task SetPermissionsAsync(string userId, List<Permission> permissions, CancellationToken ct = default);
}
