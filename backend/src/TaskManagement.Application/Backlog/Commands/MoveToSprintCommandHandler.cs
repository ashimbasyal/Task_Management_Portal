using MediatR;
using TaskManagement.Application.Backlog.Interfaces;

namespace TaskManagement.Application.Backlog.Commands;

public sealed class MoveToSprintCommandHandler(IBacklogRepository repository)
    : IRequestHandler<MoveToSprintCommand, int>
{
    public async Task<int> Handle(MoveToSprintCommand request, CancellationToken cancellationToken) =>
        await repository.MoveToSprintAsync(request, cancellationToken);
}
