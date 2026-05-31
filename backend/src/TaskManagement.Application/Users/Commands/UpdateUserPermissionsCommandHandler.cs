using MediatR;
using TaskManagement.Application.Users.Interfaces;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.Users.Commands;

public sealed class UpdateUserPermissionsCommandHandler(IUserRepository userRepository)
    : IRequestHandler<UpdateUserPermissionsCommand, List<string>>
{
    public async Task<List<string>> Handle(UpdateUserPermissionsCommand request, CancellationToken cancellationToken)
    {
        _ = await userRepository.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException($"User '{request.UserId}' not found.");

        var permissions = request.GrantedPermissions.Select(p => (Permission)p).ToList();
        await userRepository.SetPermissionsAsync(request.UserId, permissions, cancellationToken);

        return permissions.Select(p => p.ToString()).ToList();
    }
}
