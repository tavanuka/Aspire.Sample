using Microsoft.EntityFrameworkCore;
using Sample.Infrastructure.Entities;
using Sample.Infrastructure.Seeding;
using System.Runtime.InteropServices.ComTypes;

namespace Sample.Infrastructure;

public class CoreDbContext : DbContext
{
    public DbSet<Person> People { get; set; }
    
    public CoreDbContext(DbContextOptions<CoreDbContext> options)
        : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CoreDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}