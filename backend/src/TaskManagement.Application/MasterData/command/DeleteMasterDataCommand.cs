using MediatR;
using TaskManagement.Application.Common.Behaviours;

namespace TaskManagement.Application.MasterData.command
{
    public class DeleteMasterDataCommand : IRequest<APIResponse>
    {
        public int Id { get; set; }
    }
}
