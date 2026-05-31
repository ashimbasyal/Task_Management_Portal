using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Common.Behaviours;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.Users.Commands;

public sealed class UpdateUserPermissionsCommandHandler(
    UserManager<AppUser> userManager,
    IApplicationDbContext context)
    : IRequestHandler<UpdateUserPermissionsCommand, List<string>>
{
    public async Task<List<string>> Handle(UpdateUserPermissionsCommand request, CancellationToken cancellationToken)
    {
        _ = await userManager.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken)
            ?? throw new KeyNotFoundException($"User '{request.UserId}' not found.");

        var existing = await context.UserPermissions
            .Where(up => up.UserId == request.UserId)
            .ToListAsync(cancellationToken);

        context.UserPermissions.RemoveRange(existing);

        var permissions = request.GrantedPermissions.Select(p => (Permission)p).ToList();
        var newPermissions = permissions.Select(p => new UserPermission
        {
            UserId = request.UserId,
            Permission = p,
            IsGranted = true
        });

        context.UserPermissions.AddRange(newPermissions);
        await context.SaveChangesAsync(cancellationToken);

        return permissions.Select(p => p.ToString()).ToList();
    }
}
