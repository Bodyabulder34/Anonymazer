using Anonymazer.Models;
using Microsoft.EntityFrameworkCore;

namespace Anonymazer.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<AnonymizationHistory> AnonymizationHistory =>
        Set<AnonymizationHistory>();
}