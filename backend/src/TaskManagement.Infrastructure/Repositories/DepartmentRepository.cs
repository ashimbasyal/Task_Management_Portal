using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Departments.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Persistence;

namespace TaskManagement.Infrastructure.Repositories;

public class DepartmentRepository(AppDbContext db) : IDepartmentRepository
{
    public async Task<IReadOnlyList<Department>> GetAllActiveAsync(CancellationToken ct = default) =>
        await db.Departments.Where(d => d.IsActive).OrderBy(d => d.Name).ToListAsync(ct);
}
