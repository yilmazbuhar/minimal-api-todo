using Microsoft.EntityFrameworkCore;

class TodoDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public TodoDbContext(DbContextOptions<TodoDbContext> options)
        : base(options) { 
    }

    public Microsoft.EntityFrameworkCore.DbSet<TodoItem> TodoItem => Set<TodoItem>();

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