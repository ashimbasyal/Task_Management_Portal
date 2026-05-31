using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Common.Behaviours;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.BacklogTasks.command;

public class MoveToSprintCommandHandler : IRequestHandler<MoveToSprintCommand, APIResponse>
{
    private readonly IApplicationDbContext _context;
    public MoveToSprintCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<APIResponse> Handle(MoveToSprintCommand request, CancellationToken cancellationToken)
    {
        var backlogTask = await _context.BacklogTasks
            .FirstOrDefaultAsync(x => x.Id == request.BacklogTaskId, cancellationToken);

        if (backlogTask == null)
            return new APIResponse { StatusCode = 404, Message = "Backlog task not found" };

        if (backlogTask.IsMovedToSprint)
            return new APIResponse { StatusCode = 400, Message = "Already moved to sprint" };

        var sprintTask = new SprintTask
        {
            BacklogTaskId = request.BacklogTaskId,
            SprintName = request.SprintName,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Remarks = request.Remarks,
            AssigneeId = request.AssigneeId,
            StatusId = backlogTask.StatusId ?? 1,
            CreatedAt = DateTime.UtcNow
        };

        _context.SprintTasks.Add(sprintTask);
        backlogTask.IsMovedToSprint = true;
        await _context.SaveChangesAsync(cancellationToken);

        return new APIResponse { StatusCode = 200, Message = "Moved to sprint successfully", Data = sprintTask.Id };
    }
}
