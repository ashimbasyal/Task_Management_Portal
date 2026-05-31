using MediatR;
using TaskManagement.Application.Common.Behaviours;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.MasterData.Query
{
    public class GetMasterDataByTypeQuery : IRequest<APIResponse>
    {
        public MasterDataType Type { get; set; }
    }
}
