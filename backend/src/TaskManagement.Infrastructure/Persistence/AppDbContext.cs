using Microsoft.EntityFrameworkCore;

namespace TaskManagement.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
}
