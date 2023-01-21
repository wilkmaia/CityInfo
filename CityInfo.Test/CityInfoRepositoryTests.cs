using CityInfo.API.Entities;
using CityInfo.API.Exceptions;
using CityInfo.API.Models;
using CityInfo.Test.Fixtures;
using CityInfo.Test.TestData;
using Xunit.Abstractions;

namespace CityInfo.Test;

public class CityInfoRepositoryTests : IClassFixture<CityInfoRepositoryFixture>
{
    private readonly ITestOutputHelper _output;
    private readonly CityInfoRepositoryFixture _cityInfoRepositoryFixture;
    private int _newPointOfInterestId;

    public CityInfoRepositoryTests(ITestOutputHelper output, CityInfoRepositoryFixture cityInfoRepositoryFixture)
    {
        _output = output;
        _cityInfoRepositoryFixture = cityInfoRepositoryFixture;
    }

    [Fact]
    public async Task GetCities_FetchDatabaseEntries_EnsureCorrectBehavior()
    {
        var (cities, paginationMetadata) = await _cityInfoRepositoryFixture.TestCityInfoRepository.GetCities(null,
            null,
            1,
            10);

        Assert.Equal(3, cities.Count);
        Assert.Equal(3, paginationMetadata.TotalItemCount);
        Assert.Equal(1, paginationMetadata.TotalPageCount);
        Assert.Equal(10, paginationMetadata.PageSize);
        Assert.Equal(1, paginationMetadata.CurrentPage);
    }

    [Theory]
    [ClassData(typeof(CitiesTestData))]
    public async Task GetCityById_FetchDatabaseEntry_EnsureCorrectBehavior(City city)
    {
        var cityDto = await _cityInfoRepositoryFixture.TestCityInfoRepository.GetCityById(city.Id);

        Assert.Equal(city.Name, cityDto.Name);
        Assert.Equal(city.Description, cityDto.Description);
        Assert.Equal(city.Id, cityDto.Id);
        Assert.Equal(city.PointsOfInterest.Count, cityDto.NumberOfPointsOfInterest);

        foreach (var pointOfInterest in city.PointsOfInterest)
        {
            var found = false;
            foreach (var pointOfInterestDto in cityDto.PointsOfInterest)
                if (pointOfInterest.Id == pointOfInterestDto.Id)
                {
                    found = true;
                    Assert.Equal(pointOfInterest.Name, pointOfInterestDto.Name);
                    Assert.Equal(pointOfInterest.Description, pointOfInterestDto.Description);
                    break;
                }

            Assert.True(found,
                $"Point of interest {pointOfInterest.Id} not found in response for GetCityById({city.Id}).");
        }
    }

    [Theory]
    [ClassData(typeof(PointsOfInterestTestData))]
    public async Task GetPointOfInterestForCityById_FetchDatabaseEntries_EnsureCorrectBehavior(City city,
        PointOfInterest pointOfInterest)
    {
        var pointOfInterestDto = await _cityInfoRepositoryFixture.TestCityInfoRepository.GetPointOfInterestForCityById(
            city.Id,
            pointOfInterest.Id);

        Assert.Equal(pointOfInterest.Name, pointOfInterestDto.Name);
        Assert.Equal(pointOfInterest.Description, pointOfInterestDto.Description);
        Assert.Equal(pointOfInterest.Id, pointOfInterestDto.Id);
    }

    [Fact]
    public async Task CreatePointOfInterestForCity_CreateDatabaseEntry_EnsureCorrectBehavior()
    {
        var pointOfInterest = new PointOfInterestForCreationDto()
        {
            Name = "New Point of Interest",
            Description = "Description for new point of interest"
        };
        var pointOfInterestDto = await _cityInfoRepositoryFixture.TestCityInfoRepository.CreatePointOfInterestForCity(
            1,
            pointOfInterest);

        Assert.Equal(pointOfInterest.Name, pointOfInterestDto.Name);
        Assert.Equal(pointOfInterest.Description, pointOfInterestDto.Description);
        
        var newPointOfInterest = await _cityInfoRepositoryFixture.TestCityInfoRepository.GetPointOfInterestForCityById(
            1,
            pointOfInterestDto.Id);
        
        Assert.Equal(pointOfInterest.Name, newPointOfInterest.Name);
        Assert.Equal(pointOfInterest.Description, newPointOfInterest.Description);

        _newPointOfInterestId = newPointOfInterest.Id;
    }

    [Theory]
    [ClassData(typeof(PointsOfInterestTestData))]
    public async Task UpdatePointOfInterest_UpdateDatabaseEntries_EnsureCorrectBehavior(City city, PointOfInterest pointOfInterest)
    {
        var newPointOfInterestData = new PointOfInterestForUpdateDto() { Description = "Up to date description" };
        await _cityInfoRepositoryFixture.TestCityInfoRepository.UpdatePointOfInterest(
            city.Id,
            pointOfInterest.Id,
            newPointOfInterestData);

        var newPointOfInterest =
            await _cityInfoRepositoryFixture.TestCityInfoRepository.GetPointOfInterestForCityById(city.Id,
                pointOfInterest.Id);

        Assert.Equal(pointOfInterest.Name, newPointOfInterest.Name);
        Assert.Equal(newPointOfInterestData.Description, newPointOfInterest.Description);
    }

    [Fact]
    public async Task DeletePointOfInterest_DeleteDatabaseEntry_EnsureCorrectBehavior()
    {
        await _cityInfoRepositoryFixture.TestCityInfoRepository.DeletePointOfInterest(1, _newPointOfInterestId);

        await Assert.ThrowsAsync<PointOfInterestNotFoundException>(() =>
            _cityInfoRepositoryFixture.TestCityInfoRepository.GetPointOfInterestForCityById(1, _newPointOfInterestId));
    }
}