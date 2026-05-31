using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.MasterData.command;
using TaskManagement.Application.MasterData.Query;
using TaskManagement.Domain.Enums;

namespace TaskManagement.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MasterDataController(IMediator mediator) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Create(CreateMasterDataCommand command)
        {
            var response = await mediator.Send(command);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("{type}")]
        public async Task<IActionResult> GetByType(MasterDataType type)
        {
            var response = await mediator.Send(new GetMasterDataByTypeQuery { Type = type });
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateMasterDataCommand command)
        {
            var response = await mediator.Send(command);
            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await mediator.Send(new DeleteMasterDataCommand { Id = id });
            return StatusCode(response.StatusCode, response);
        }
    }
}
