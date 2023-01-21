using System.Collections;
using CityInfo.API.Data;
using CityInfo.API.Entities;

namespace CityInfo.Test.TestData;

public class CitiesTestData : TheoryData<City>
{
    public CitiesTestData()
    {
        foreach (var city in SeedData.Cities)
        {
            Add(city);
        }
    }
}