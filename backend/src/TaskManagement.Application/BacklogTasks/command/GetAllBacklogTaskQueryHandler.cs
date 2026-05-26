using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.BacklogTasks.DTOs;
using TaskManagement.Application.Common.Behaviours;

namespace TaskManagement.Application.BacklogTasks.command
{
    public class GetAllBacklogTaskQueryHandler : IRequestHandler<GetAllBacklogTaskQuery, APIResponse>
    {
        private readonly IApplicationDbContext _context;
        public GetAllBacklogTaskQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<APIResponse> Handle(GetAllBacklogTaskQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var backlogTasks = await _context.BacklogTasks
                    .Select(x => new BacklogTaskDto
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Description = x.Description,
                        RequestedBy = x.RequestedBy,
                        GitLabLink = x.GitLabLink,
                        Remarks = x.Remarks,
                        PriorityId = x.PriorityId,
                        StatusId = x.StatusId,
                        DepartmentId = x.DepartmentId,
                        IsMovedToSprint = x.IsMovedToSprint,
                        CreatedAt = x.CreatedAt,
                        CreatedBy = x.CreatedBy,
                        UpdatedAt = x.UpdatedAt,
                        UpdatedBy = x.UpdatedBy
                    })
                    .ToListAsync(cancellationToken);

                return new APIResponse
                {
                    StatusCode = 200,
                    Message = "Backlog tasks retrieved successfully.",
                    Data = backlogTasks,
                    Error = null
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    StatusCode = 500,
                    Message = "Failed to retrieve backlog tasks.",
                    Data = null,
                    Error = ex.Message
                };
            }
        }
    }
}
