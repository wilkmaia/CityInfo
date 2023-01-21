using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Data;

public static class SeedData
{
    public static readonly List<City> Cities = new()
    {
        new City("Teresina")
        {
            Id = 1,
            Description = "Capital do Piauí",
            PointsOfInterest = new List<PointOfInterest>()
            {
                new PointOfInterest("POI-1")
                {
                    Id = 1,
                    Description = "Point of Interest 1",
                },
                new PointOfInterest("POI-2")
                {
                    Id = 2,
                    Description = "Point of Interest 2",
                },
            },
        },
        new City("Timon")
        {
            Id = 2,
            Description = "Quintal de Teresina",
            PointsOfInterest = new List<PointOfInterest>()
            {
                new PointOfInterest("POI-3")
                {
                    Id = 3,
                    Description = "Point of Interest 3",
                },
                new PointOfInterest("POI-4")
                {
                    Id = 4,
                    Description = "Point of Interest 4",
                },
            },
        },
        new City("São João dos Patos")
        {
            Id = 3,
            PointsOfInterest = new List<PointOfInterest>()
            {
                new PointOfInterest("POI-5")
                {
                    Id = 5,
                    Description = "Point of Interest 5",
                },
                new PointOfInterest("POI-6")
                {
                    Id = 6,
                    Description = "Point of Interest 6",
                },
            },
        },
    };
    
    public static void Initialize(CityInfoContext context)
    {
        if (context.Cities.Any() && context.PointsOfInterest.Any())
        {
            return;
        }
        
        context.Cities.AddRange(Cities);
        context.SaveChanges();
    }
}