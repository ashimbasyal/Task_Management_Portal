using MediatR;
using TaskManagement.Application.Users.Interfaces;

namespace TaskManagement.Application.Users.Queries;

public sealed class GetUserPermissionsQueryHandler(IUserRepository userRepository)
    : IRequestHandler<GetUserPermissionsQuery, List<string>>
{
    public async Task<List<string>> Handle(GetUserPermissionsQuery request, CancellationToken cancellationToken) =>
        await userRepository.GetGrantedPermissionsAsync(request.UserId, cancellationToken);
}
