using MediatR;
using TaskManagement.Application.Common.Behaviours;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.MasterData.command
{
    public class DeleteMasterDataCommand : IRequest<APIResponse>
    {
        public int Id { get; set; }
        public MasterDataType Type { get; set; }
    }
}
