using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Common.Behaviours;

namespace TaskManagement.Application.Users.Queries;

public sealed class GetUserPermissionsQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetUserPermissionsQuery, List<string>>
{
    public async Task<List<string>> Handle(GetUserPermissionsQuery request, CancellationToken cancellationToken) =>
        await context.UserPermissions
            .Where(up => up.UserId == request.UserId && up.IsGranted)
            .Select(up => up.Permission.ToString())
            .ToListAsync(cancellationToken);
}
