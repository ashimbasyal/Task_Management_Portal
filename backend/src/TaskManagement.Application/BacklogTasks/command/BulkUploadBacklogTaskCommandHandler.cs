using MediatR;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using TaskManagement.Application.Common.Behaviours;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.BacklogTasks.command
{
    public class BulkUploadBacklogTaskCommandHandler
        : IRequestHandler<BulkUploadBacklogTaskCommand, APIResponse>
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
                var skippedRows = new List<int>();

                var currentMaxSN = await _context.BacklogTasks
                    .MaxAsync(x => (int?)x.SN, cancellationToken) ?? 0;

                var existingTitles = await _context.BacklogTasks
                    .Select(x => x.Title.ToLower())
                    .ToListAsync(cancellationToken);

                var excelTitles = new HashSet<string>();

                using var stream = new MemoryStream();
                await request.File.CopyToAsync(stream, cancellationToken);

                using var package = new ExcelPackage(stream);
                var worksheet = package.Workbook.Worksheets[0];

                int rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)
                {
                    var title = worksheet.Cells[row, 1].Text?.Trim();

                    if (string.IsNullOrWhiteSpace(title))
                    {
                        skippedRows.Add(row);
                        continue;
                    }

                    var normalizedTitle = title.ToLower();

                    if (existingTitles.Contains(normalizedTitle))
                    {
                        skippedRows.Add(row);
                        continue;
                    }

                    if (!excelTitles.Add(normalizedTitle))
                    {
                        skippedRows.Add(row);
                        continue;
                    }

                    var task = new BacklogTask
                    {
                        SN = ++currentMaxSN,
                        Title = title,
                        Description = worksheet.Cells[row, 2].Text,
                        RequestedBy = worksheet.Cells[row, 3].Text,
                        GitLabLink = worksheet.Cells[row, 4].Text,
                        Remarks = worksheet.Cells[row, 5].Text,

                        PriorityId = int.TryParse(
                            worksheet.Cells[row, 6].Text,
                            out var priorityId) ? priorityId : null,

                        StatusId = int.TryParse(
                            worksheet.Cells[row, 7].Text,
                            out var statusId) ? statusId : null,

                        DepartmentId = int.TryParse(
                            worksheet.Cells[row, 8].Text,
                            out var departmentId) ? departmentId : null,

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
                    Message = $"Upload completed. Inserted: {backlogTasks.Count}, Skipped: {skippedRows.Count}",
                    Data = new
                    {
                        Inserted = backlogTasks.Count,
                        Skipped = skippedRows.Count
                    }
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