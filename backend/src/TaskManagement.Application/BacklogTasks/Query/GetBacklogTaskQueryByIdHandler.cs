using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.BacklogTasks.DTOs;
using TaskManagement.Application.Common.Behaviours;

namespace TaskManagement.Application.BacklogTasks.Query
{
    public class GetBacklogTaskQueryByIdHandler : IRequestHandler<GetBacklogTaskByIdQuery, APIResponse>
    {
        private readonly IApplicationDbContext _context;
        public GetBacklogTaskQueryByIdHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<APIResponse> Handle(GetBacklogTaskByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var backlogTask = await _context.BacklogTasks
                    .Where(x => x.Id == request.Id)
                    .Select(x => new BacklogTaskDto
                    {
                        Id = x.Id,
                        SN = x.SN,
                        Title = x.Title,
                        Description = x.Description,
                        RequestedBy = x.RequestedBy,
                        GitLabLink = x.GitLabLink,
                        Remarks = x.Remarks,
                        PriorityId = x.PriorityId,
                        PriorityName = x.Priority != null ? x.Priority.Name : null,
                        StatusId = x.StatusId,
                        StatusName = x.Status != null ? x.Status.Name : null,
                        DepartmentId = x.DepartmentId,
                        DepartmentName = x.Department != null ? x.Department.Name : null,
                        IsMovedToSprint = x.IsMovedToSprint,
                        CreatedAt = x.CreatedAt,
                        CreatedBy = x.CreatedBy,
                        UpdatedAt = x.UpdatedAt,
                        UpdatedBy = x.UpdatedBy
                    })
                    .FirstOrDefaultAsync(cancellationToken);

                if (backlogTask == null)
                {
                    return new APIResponse
                    {
                        StatusCode = 404,
                        Message = "Backlog task not found.",
                        Data = null,
                        Error = null
                    };
                }

                return new APIResponse
                {
                    StatusCode = 200,
                    Message = "Backlog task retrieved successfully.",
                    Data = backlogTask,
                    Error = null
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    StatusCode = 500,
                    Message = "Failed to retrieve backlog task.",
                    Data = null,
                    Error = ex.Message
                };
            }
        }
    }
}

