using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Departments.Interfaces;

public interface IDepartmentRepository
{
    Task<IReadOnlyList<Department>> GetAllActiveAsync(CancellationToken ct = default);
}
