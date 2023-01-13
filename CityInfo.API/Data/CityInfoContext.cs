using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Data;

public class CityInfoContext : DbContext
{
    public CityInfoContext(DbContextOptions<CityInfoContext> options) : base(options)
    {
    }

    public DbSet<City> Cities => Set<City>();
    public DbSet<PointOfInterest> PointsOfInterest => Set<PointOfInterest>();
}