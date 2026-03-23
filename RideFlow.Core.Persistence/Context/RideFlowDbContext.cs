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
}