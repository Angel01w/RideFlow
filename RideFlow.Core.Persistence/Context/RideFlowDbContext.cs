using Microsoft.EntityFrameworkCore;
using RideFlow.Core.Domain.Entities;

namespace RideFlow.Core.Persistence.Context;

public class RideFlowDbContext : DbContext
{
    public RideFlowDbContext(DbContextOptions<RideFlowDbContext> options) : base(options)
    {
    }

    public DbSet<Driver> Drivers => Set<Driver>();
}