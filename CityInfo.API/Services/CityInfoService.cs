using System.Drawing;
using CityInfo.API.Data;
using CityInfo.API.Exceptions;
using CityInfo.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Query;

namespace CityInfo.API.Services;

public class CityInfoService
{
    private readonly DatabaseContext _dbContext;

    public CityInfoService(DatabaseContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public Task<List<CityDto>> GetAllCities()
    {
        return _dbContext.Cities
            .Include(c => c.PointsOfInterest)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<CityDto> GetCityById(int cityId, bool track = false)
    {
        var query = track ? _dbContext.Cities : _dbContext.Cities.AsNoTracking();
        var city = await query
            .Include(c => c.PointsOfInterest)
            .SingleOrDefaultAsync(c => c.Id == cityId);
        if (city == null)
        {
            throw new CityNotFoundException("Id", cityId.ToString());
        }

        return city;
    }

    public async Task<List<PointOfInterestDto>> GetAllPointsOfInterestForCity(int cityId)
    {
        var city = await _dbContext.Cities
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == cityId);
        if (city == null)
        {
            throw new CityNotFoundException("Id", cityId.ToString());
        }

        return city.PointsOfInterest.ToList();
    }

    public async Task<PointOfInterestDto> GetPointOfInterestById(int poiId)
    {
        var pointOfInterest = await _dbContext.PointsOfInterest
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == poiId);
        if (pointOfInterest == null)
        {
            throw new PointOfInterestNotFoundException("Id", poiId.ToString());
        }

        return pointOfInterest;
    }

    public async Task<PointOfInterestDto> GetPointOfInterestForCityById(int cityId, int poiId, bool track = false)
    {
        var query = track ? _dbContext.Cities : _dbContext.Cities.AsNoTracking();
        var city = await query
            .Include(c => c.PointsOfInterest)
            .FirstOrDefaultAsync(c => c.Id == cityId);
        if (city == null)
        {
            throw new CityNotFoundException("Id", cityId.ToString());
        }

        var pointOfInterest = city.PointsOfInterest.FirstOrDefault(p => p.Id == poiId);
        if (pointOfInterest == null)
        {
            throw new PointOfInterestNotFoundException("Id", poiId.ToString());
        }

        return pointOfInterest;
    }

    public async Task<PointOfInterestDto> CreatePointOfInterestForCity(int cityId, PointOfInterestForCreationDto poi)
    {
        var city = await GetCityById(cityId, true);
        if (city == null)
        {
            throw new CityNotFoundException("Id", cityId.ToString());
        }
        
        var entry = new PointOfInterestDto()
        {
            Description = poi.Description,
            Name = poi.Name,
        };
        var pointOfInterest = _dbContext.PointsOfInterest
            .Add(entry);
        city.PointsOfInterest.Add(pointOfInterest.Entity);
        Console.WriteLine(city.NumberOfPointsOfInterest);
        Console.WriteLine(pointOfInterest.Entity);
        await _dbContext.SaveChangesAsync();

        return pointOfInterest.Entity;
    }

    public async Task UpdatePointOfInterest(int cityId, int poiId, PointOfInterestForUpdateDto poi)
    {
        
        var pointOfInterest = await GetPointOfInterestForCityById(cityId, poiId, true);
        pointOfInterest.Name = poi.Name;
        pointOfInterest.Description = poi.Description;
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdatePointOfInterest(PointOfInterestDto pointOfInterest, PointOfInterestForUpdateDto poi)
    {
        
        pointOfInterest.Name = poi.Name;
        pointOfInterest.Description = poi.Description;
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeletePointOfInterest(int cityId, int poiId)
    {
        
        var pointOfInterest = await GetPointOfInterestForCityById(cityId, poiId, true);
        _dbContext.PointsOfInterest.Remove(pointOfInterest);
        await _dbContext.SaveChangesAsync();
    }
}