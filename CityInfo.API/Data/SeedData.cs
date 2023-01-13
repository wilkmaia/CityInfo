using CityInfo.API.Models;

namespace CityInfo.API.Data;

public static class SeedData
{
    public static void Initialize(DatabaseContext context)
    {
        if (context.Cities.Any() && context.PointsOfInterest.Any())
        {
            return;
        }
        
        var cities = new List<CityDto>()
        {
            new CityDto()
            {
                Id = 1,
                Name = "Teresina",
                Description = "Capital do Piauí",
                PointsOfInterest = new List<PointOfInterestDto>()
                {
                    new PointOfInterestDto()
                    {
                        Id = 1,
                        Name = "POI-1",
                        Description = "Point of Interest 1",
                    },
                    new PointOfInterestDto()
                    {
                        Id = 2,
                        Name = "POI-2",
                        Description = "Point of Interest 2",
                    },
                },
            },
            new CityDto()
            {
                Id = 2,
                Name = "Timon",
                Description = "Quintal de Teresina",
                PointsOfInterest = new List<PointOfInterestDto>()
                {
                    new PointOfInterestDto()
                    {
                        Id = 3,
                        Name = "POI-3",
                        Description = "Point of Interest 3",
                    },
                    new PointOfInterestDto()
                    {
                        Id = 4,
                        Name = "POI-4",
                        Description = "Point of Interest 4",
                    },
                },
            },
            new CityDto()
            {
                Id = 3,
                Name = "São João dos Patos",
                PointsOfInterest = new List<PointOfInterestDto>()
                {
                    new PointOfInterestDto()
                    {
                        Id = 5,
                        Name = "POI-5",
                        Description = "Point of Interest 5",
                    },
                    new PointOfInterestDto()
                    {
                        Id = 6,
                        Name = "POI-6",
                        Description = "Point of Interest 6",
                    },
                },
            },
        };
        
        context.Cities.AddRange(cities);
        context.SaveChanges();
    }
}