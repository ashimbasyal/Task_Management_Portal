using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Common.Behaviours;

namespace TaskManagement.Application.Departments.command
{
    public class UpdateDepartmentCommandHandler : IRequestHandler<UpdateDepartmentCommand, APIResponse>
    {
        private readonly IApplicationDbContext _context;
        public UpdateDepartmentCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<APIResponse> Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
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

                department.Name = request.Name;

                await _context.SaveChangesAsync(cancellationToken);

                return new APIResponse
                {
                    StatusCode = 200,
                    Message = "Department updated successfully",
                    Data = new
                    {
                        department.Id,
                        department.Name
                    }
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    StatusCode = 500,
                    Message = "Failed to update department",
                    Error = ex.Message
                };
            }
        }
    }
}
