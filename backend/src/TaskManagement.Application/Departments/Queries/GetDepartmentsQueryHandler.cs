using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Common.Behaviours;
using TaskManagement.Application.Departments.DTOs;

namespace TaskManagement.Application.Departments.Queries;

public class GetDepartmentsQueryHandler : IRequestHandler<GetDepartmentsQuery, APIResponse>
{
    private readonly IApplicationDbContext _context;
    public GetDepartmentsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<APIResponse> Handle(GetDepartmentsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var departments = await _context.Departments
                .Where(d => d.IsActive)
                .OrderBy(d => d.Name)
                .Select(d => new DepartmentDto(d.Id, d.Name))
                .ToListAsync(cancellationToken);

            return new APIResponse
            {
                StatusCode = 200,
                Message = "Departments retrieved successfully",
                Data = departments
            };
        }
        catch (Exception ex)
        {
            return new APIResponse
            {
                StatusCode = 500,
                Message = "Failed to retrieve departments",
                Error = ex.Message
            };
        }
    }
}
