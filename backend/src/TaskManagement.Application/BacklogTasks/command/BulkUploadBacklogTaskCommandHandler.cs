using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using OfficeOpenXml;
using TaskManagement.Application.Common.Behaviours;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.BacklogTasks.command
{
    public class BulkUploadBacklogTaskCommandHandler : IRequestHandler<BulkUploadBacklogTaskCommand, APIResponse>
    {
        private readonly IApplicationDbContext _context;
        public BulkUploadBacklogTaskCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<APIResponse> Handle(BulkUploadBacklogTaskCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.File == null || request.File.Length == 0)
                {
                    return new APIResponse
                    {
                        StatusCode = 400,
                        Message = "Please upload a valid Excel file."
                    };
                }


                var backlogTasks = new List<BacklogTask>();

                using var stream = new MemoryStream();

                await request.File.CopyToAsync(stream, cancellationToken);

                using var package = new ExcelPackage(stream);

                var worksheet = package.Workbook.Worksheets[0];

                int rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)
                {
                    var task = new BacklogTask
                    {
                        Title = worksheet.Cells[row, 1].Text,
                        Description = worksheet.Cells[row, 2].Text,
                        RequestedBy = worksheet.Cells[row, 3].Text,
                        GitLabLink = worksheet.Cells[row, 4].Text,
                        Remarks = worksheet.Cells[row, 5].Text,

                        PriorityId = int.TryParse(
                            worksheet.Cells[row, 6].Text,
                            out var priorityId)
                            ? priorityId
                            : null,

                        StatusId = int.TryParse(
                            worksheet.Cells[row, 7].Text,
                            out var statusId)
                            ? statusId
                            : null,

                        DepartmentId = int.TryParse(
                            worksheet.Cells[row, 8].Text,
                            out var departmentId)
                            ? departmentId
                            : null,

                        CreatedBy = worksheet.Cells[row, 9].Text,

                        CreatedAt = DateTime.UtcNow,
                        IsMovedToSprint = false
                    };

                    backlogTasks.Add(task);
                }

                _context.BacklogTasks.AddRange(backlogTasks);

                await _context.SaveChangesAsync(cancellationToken);

                return new APIResponse
                {
                    StatusCode = 200,
                    Message = $"{backlogTasks.Count} tasks uploaded successfully.",
                    Data = backlogTasks.Count
                };
            }
            catch (Exception ex)
            {
                return new APIResponse
                {
                    StatusCode = 500,
                    Message = "Bulk upload failed.",
                    Error = ex.Message
                };
            }
        }
    }
}
