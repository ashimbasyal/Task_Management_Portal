using MediatR;
using TaskManagement.Application.Common.Behaviours;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Departments.command
{
    public class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand, APIResponse>
    {
        private readonly IApplicationDbContext _context;
        public CreateDepartmentCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<APIResponse> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var department = new Department
                {
                    Name = request.Name,
                    IsActive = true
                };

                _context.Departments.Add(department);

                await _context.SaveChangesAsync(cancellationToken);

                return new APIResponse
                {
                    StatusCode = 201,
                    Message = "Department created successfully",
                    Data = department,
                    Error = null
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    StatusCode = 500,
                    Message = "Failed to create department",
                    Data = null,
                    Error = ex.Message
                };
            }
        }
    }
}
