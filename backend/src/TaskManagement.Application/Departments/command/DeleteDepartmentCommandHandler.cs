using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Common.Behaviours;

namespace TaskManagement.Application.Departments.command
{
    public class DeleteDepartmentCommandHandler : IRequestHandler<DeleteDepartmentCommand, APIResponse>
    {
        private readonly IApplicationDbContext _context;
        public DeleteDepartmentCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<APIResponse> Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var department = await _context.Departments
                    .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (department == null)
                {
                    return new APIResponse
                    {
                        StatusCode = 404,
                        Message = "Department not found"
                    };
                }

                department.IsActive = false;
                await _context.SaveChangesAsync(cancellationToken);

                return new APIResponse
                {
                    StatusCode = 200,
                    Message = "Department deactivated successfully"
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    StatusCode = 500,
                    Message = "Failed to delete department",
                    Error = ex.Message
                };
            }
        }
    }
}
