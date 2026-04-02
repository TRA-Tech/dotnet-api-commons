using Microsoft.EntityFrameworkCore;

namespace ApiCommons.Tests.Infrastructure;

internal class TestItem
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int SortOrder { get; set; }
}

internal class TestDbContext(DbContextOptions<TestDbContext> options) : DbContext(options)
{
    public DbSet<TestItem> Items => Set<TestItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TestItem>().HasKey(x => x.Id);
    }
}
