using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using TaskManagement.Application.AuditLogs.Command;
using TaskManagement.Application.BacklogTasks.command;
using TaskManagement.Application.BacklogTasks.Query;

namespace TaskManagement.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BacklogTaskController(IMediator mediator) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> BacklogTask(
          CreateBacklogTaskCommand command)
        {
            var response = await mediator.Send(command);

            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateBacklogTask(int id, UpdateBacklogTaskCommand command)
        {
            command.Id = id;

            var response = await mediator.Send(command);

            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetBacklogTaskById(int id)
        {
            var response = await mediator.Send(new GetBacklogTaskByIdQuery { Id = id });

            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteBacklogTask(int id)
        {
            var response = await mediator.Send(new DeleteBacklogTaskCommand { Id = id });

            return StatusCode(response.StatusCode, response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBacklogTasks(
            [FromQuery] int? priorityId,
            [FromQuery] int? statusId,
            [FromQuery] int? departmentId)
        {
            var response = await mediator.Send(new GetAllBacklogTaskQuery
            {
                PriorityId = priorityId,
                StatusId = statusId,
                DepartmentId = departmentId
            });

            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("download-sample")]
        public async Task<IActionResult> DownloadSample()
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Backlog");

            worksheet.Cells[1, 1].Value = "Title";
            worksheet.Cells[1, 2].Value = "Description";
            worksheet.Cells[1, 3].Value = "RequestedBy";
            worksheet.Cells[1, 4].Value = "GitLabLink";
            worksheet.Cells[1, 5].Value = "Remarks";
            worksheet.Cells[1, 6].Value = "PriorityId";
            worksheet.Cells[1, 7].Value = "StatusId";
            worksheet.Cells[1, 8].Value = "DepartmentId";
            worksheet.Cells[1, 9].Value = "CreatedBy";

            worksheet.Cells[1, 1, 1, 9].Style.Font.Bold = true;
            worksheet.Cells[1, 1, 1, 9].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            worksheet.Cells[1, 1, 1, 9].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
            worksheet.Cells.AutoFitColumns();

            var stream = new MemoryStream();
            await package.SaveAsAsync(stream);
            stream.Position = 0;

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "backlog_sample.xlsx");
        }

        [HttpPost("bulk-upload")]
        public async Task<IActionResult> BulkUploadBacklogTask(
        IFormFile file)
        {
            var response = await mediator.Send(
                new BulkUploadBacklogTaskCommand
                {
                    File = file
                });

            return StatusCode(response.StatusCode, response);
        }
    }
}
