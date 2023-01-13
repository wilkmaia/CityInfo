using CityInfo.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Data;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    public DbSet<CityDto> Cities => Set<CityDto>();
    public DbSet<PointOfInterestDto> PointsOfInterest => Set<PointOfInterestDto>();
}