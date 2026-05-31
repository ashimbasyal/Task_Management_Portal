using MediatR;
using TaskManagement.Application.Common.Behaviours;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.MasterData.command
{
    public class CreateMasterDataCommand : IRequest<APIResponse>
    {
        public MasterDataType Type { get; set; }
        public string Value { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
    }
}
