using MediatR;
using TaskManagement.Application.Common.Behaviours;

namespace TaskManagement.Application.MasterData.command
{
    public class UpdateMasterDataCommand : IRequest<APIResponse>
    {
        public int Id { get; init; }
        public string? Value { get; init; }
        public int DisplayOrder { get; init; }
    }
}
