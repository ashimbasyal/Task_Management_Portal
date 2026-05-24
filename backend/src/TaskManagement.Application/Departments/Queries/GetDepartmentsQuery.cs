using MediatR;
using TaskManagement.Application.Departments.DTOs;

namespace TaskManagement.Application.Departments.Queries;

public record GetDepartmentsQuery : IRequest<IReadOnlyList<DepartmentDto>>;
