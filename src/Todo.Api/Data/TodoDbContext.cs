using Microsoft.EntityFrameworkCore;

namespace Todo.Api;

public class TodoDbContext : DbContext
{
    public TodoDbContext(DbContextOptions<TodoDbContext> options)
        : base(options)
    {
        if (!this.Database.EnsureCreated())
            this.Database.Migrate();
    }

    public DbSet<TodoItem> TodoItem => Set<TodoItem>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TodoItem>()
            .ToTable("TodoItems")
            .Ignore("TotalAmount")
            .HasKey("Id");

        base.OnModelCreating(modelBuilder);
    }
}