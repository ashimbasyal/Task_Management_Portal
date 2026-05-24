using MediatR;
using Microsoft.AspNetCore.Identity;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Users.Commands;

public sealed class DeleteUserCommandHandler(UserManager<AppUser> userManager)
    : IRequestHandler<DeleteUserCommand>
{
    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId)
            ?? throw new KeyNotFoundException($"User '{request.UserId}' not found.");

        var result = await userManager.DeleteAsync(user);

        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));
    }
}
