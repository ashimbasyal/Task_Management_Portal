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
                var query = _context.BacklogTasks.AsQueryable();

                if (request.PriorityId.HasValue)
                    query = query.Where(x => x.PriorityId == request.PriorityId);

                if (request.StatusId.HasValue)
                    query = query.Where(x => x.StatusId == request.StatusId);

                if (request.DepartmentId.HasValue)
                    query = query.Where(x => x.DepartmentId == request.DepartmentId);

                var backlogTasks = await query
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
