//using MediatR;
//using TaskManagement.Application.Backlog.Interfaces;

//namespace TaskManagement.Application.BacklogTasks.command;

//public sealed class MoveToSprintCommandHandler(IBacklogRepository repository)
//    : IRequestHandler<MoveToSprintCommand, int>
//{
//    public async Task<int> Handle(MoveToSprintCommand request, CancellationToken cancellationToken) =>
//        await repository.MoveToSprintAsync(request, cancellationToken);
//}
