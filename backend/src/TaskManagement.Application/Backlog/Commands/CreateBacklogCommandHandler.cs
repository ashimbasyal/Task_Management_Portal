using MediatR;
using TaskManagement.Application.Backlog.Interfaces;

namespace TaskManagement.Application.Backlog.Commands;

public sealed class CreateBacklogCommandHandler(IBacklogRepository repository)
    : IRequestHandler<CreateBacklogCommand, int>
{
    public async Task<int> Handle(CreateBacklogCommand request, CancellationToken cancellationToken) =>
        await repository.CreateAsync(request, cancellationToken);
}
