using CityInfo.API.Data;
using CityInfo.API.Entities;

namespace CityInfo.Test.TestData;

public class PointsOfInterestTestData : TheoryData<City, PointOfInterest>
{
    public PointsOfInterestTestData()
    {
        foreach (var city in SeedData.Cities)
        {
            foreach (var pointOfInterest in city.PointsOfInterest)
            {
                Add(city, pointOfInterest);
            }
        }
    }
}
