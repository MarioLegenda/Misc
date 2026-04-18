using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<Employee> Employees { get; set; } 
    
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(
            x => x.MigrationsHistoryTable("__EFMigrationsHistory", "employees")
        );
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("employees");
        
        modelBuilder.Entity<Employee>()
            .Property(e => e.Gender)
            .HasConversion(
                v => v == EmployeeGender.M ? "M" : "F",
                v => v == "M" ? EmployeeGender.M : EmployeeGender.F
            );
    }
}