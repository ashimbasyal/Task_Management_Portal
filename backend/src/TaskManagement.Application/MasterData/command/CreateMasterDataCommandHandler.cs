using MediatR;
using TaskManagement.Application.Common.Behaviours;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.MasterData.command
{
    public class CreateMasterDataCommandHandler : IRequestHandler<CreateMasterDataCommand, APIResponse>
    {
        private readonly IApplicationDbContext _context;
        public CreateMasterDataCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<APIResponse> Handle(CreateMasterDataCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.Type == MasterDataType.Assignee)
                {
                    return new APIResponse
                    {
                        StatusCode = 400,
                        Message = "Cannot create assignee from master data. Use User Management instead."
                    };
                }

                object entry = request.Type switch
                {
                    MasterDataType.Status =>
                        new Status { Name = request.Value, IsActive = true },

                    MasterDataType.Priority =>
                        new Priority { Name = request.Value, IsActive = true },

                    MasterDataType.SprintStatusTrigger =>
                        new SprintStatusTrigger { Name = request.Value, IsActive = true },

                    MasterDataType.Department =>
                        new Department { Name = request.Value, IsActive = true },

                    _ => throw new ArgumentException($"Invalid master data type: {request.Type}")
                };

                switch (entry)
                {
                    case Status s:
                        _context.Statuses.Add(s);
                        break;
                    case Priority p:
                        _context.Priorities.Add(p);
                        break;
                    case SprintStatusTrigger sst:
                        _context.SprintStatuses.Add(sst);
                        break;
                    case Department d:
                        _context.Departments.Add(d);
                        break;
                }

                await _context.SaveChangesAsync(cancellationToken);

                return new APIResponse
                {
                    StatusCode = 201,
                    Message = "Master data created successfully",
                    Data = entry,
                    Error = null
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    StatusCode = 500,
                    Message = "Failed to create master data",
                    Data = null,
                    Error = ex.Message
                };
            }
        }
    }
}
