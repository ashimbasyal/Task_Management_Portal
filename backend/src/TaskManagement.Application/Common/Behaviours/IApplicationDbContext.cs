using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Common.Behaviours
{
    public interface IApplicationDbContext
    {
        DbSet<AuditLog> AuditLogs { get; }
        DbSet<BacklogTask> BacklogTasks { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
