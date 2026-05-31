using MediatR;
using TaskManagement.Application.Common.Behaviours;

namespace TaskManagement.Application.Departments.Queries;

public record GetDepartmentsQuery : IRequest<APIResponse>;
