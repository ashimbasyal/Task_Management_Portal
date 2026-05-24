using MediatR;
using TaskManagement.Application.Departments.DTOs;
using TaskManagement.Application.Departments.Interfaces;

namespace TaskManagement.Application.Departments.Queries;

public sealed class GetDepartmentsQueryHandler(IDepartmentRepository repo)
    : IRequestHandler<GetDepartmentsQuery, IReadOnlyList<DepartmentDto>>
{
    public async Task<IReadOnlyList<DepartmentDto>> Handle(GetDepartmentsQuery request, CancellationToken cancellationToken)
    {
        var departments = await repo.GetAllActiveAsync(cancellationToken);
        return departments.Select(d => new DepartmentDto(d.Id, d.Name)).ToList();
    }
}
