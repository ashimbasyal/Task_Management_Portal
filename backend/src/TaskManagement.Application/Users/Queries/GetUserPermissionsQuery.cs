using MediatR;

namespace TaskManagement.Application.Users.Queries;

public record GetUserPermissionsQuery(string UserId) : IRequest<List<string>>;
