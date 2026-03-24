using Microsoft.EntityFrameworkCore;
using RideFlow.Core.Domain.Entities;

namespace RideFlow.Core.Persistence.Context;

public class RideFlowDbContext : DbContext
{
    public RideFlowDbContext(DbContextOptions<RideFlowDbContext> options) : base(options)
    {
    }

    public DbSet<Driver> Drivers => Set<Driver>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Route> Routes => Set<Route>();
    public DbSet<RouteAssignment> RouteAssignments => Set<RouteAssignment>();
    public DbSet<Attendance> Attendances => Set<Attendance>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Route>()
            .HasOne(r => r.Driver)
            .WithMany(d => d.Routes)
            .HasForeignKey(r => r.DriverId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RouteAssignment>()
            .HasOne(ra => ra.Route)
            .WithMany(r => r.Assignments)
            .HasForeignKey(ra => ra.RouteId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RouteAssignment>()
            .HasOne(ra => ra.Employee)
            .WithMany(e => e.Assignments)
            .HasForeignKey(ra => ra.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}