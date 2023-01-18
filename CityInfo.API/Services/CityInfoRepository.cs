using AutoMapper;
using CityInfo.API.Data;
using CityInfo.API.Entities;
using CityInfo.API.Exceptions;
using CityInfo.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Services;

public class CityInfoRepository
{
    private readonly CityInfoContext _context;
    private readonly IMapper _mapper;
    private readonly CacheService _cache;

    public CityInfoRepository(CityInfoContext context, IMapper mapper, CacheService cache)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    private async Task UpdateCityInCache(int cityId)
    {
        try
        {
            var city = await GetCityById(cityId, true);
            await _cache.StoreCity(city);
        }
        catch
        {
            // ignored
        }
    }

    private async Task UpdatePointOfInterestInCache(int cityId, int poiId)
    {
        try
        {
            var pointOfInterest = await GetPointOfInterestForCityById(cityId, poiId, true);
            await _cache.StorePointOfInterest(cityId, pointOfInterest);
        }
        catch
        {
            // ignored
        }
    }

    public async Task<(List<CityDto>, PaginationMetadata)> GetCities(string? nameFilter,
        string? searchQuery,
        int pageNumber,
        int pageSize)
    {
        var citiesQuery = _context.Cities
            .Include(c => c.PointsOfInterest)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(nameFilter))
        {
            citiesQuery = citiesQuery.Where(c => c.Name == nameFilter);
        }

        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            var s = searchQuery.Trim();
            citiesQuery = citiesQuery
                .Where(c => c.Name.Contains(s) || (c.Description != null && c.Description.Contains(s)));
        }

        citiesQuery = citiesQuery
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize);

        int totalItemCount = await _context.Cities.CountAsync();
        PaginationMetadata paginationMetadata = new(totalItemCount, pageSize, pageNumber);

        return (
            _mapper.Map<List<CityDto>>(await citiesQuery.ToListAsync()),
            paginationMetadata
        );
    }

    public async Task<CityDto> GetCityById(int cityId, bool bypassCache = false)
    {
        if (!bypassCache)
        {
            var cachedCity = await _cache.GetCity(cityId);
            
#pragma warning disable CS4014
            // Intentionally not _await_ing
            UpdateCityInCache(cityId);
#pragma warning restore CS4014
            
            if (cachedCity != null)
            {
                return cachedCity;
            }
        }

        var city = await _context.Cities
            .AsNoTracking()
            .Include(c => c.PointsOfInterest)
            .SingleOrDefaultAsync(c => c.Id == cityId);
        if (city == null)
        {
            throw new CityNotFoundException("Id", cityId.ToString());
        }

        return _mapper.Map<CityDto>(city);
    }

    public async Task<PointOfInterestDto> GetPointOfInterestForCityById(int cityId, int poiId, bool bypassCache = false)
    {
        if (!bypassCache)
        {
            var cachedPointOfInterest = await _cache.GetPointOfInterest(cityId, poiId);

#pragma warning disable CS4014
            // Intentionally not _await_ing
            UpdatePointOfInterestInCache(cityId, poiId);
#pragma warning restore CS4014

            if (cachedPointOfInterest != null)
            {
                return cachedPointOfInterest;
            }
        }
        
        var pointOfInterest = await _context.PointsOfInterest
            .AsNoTracking()
            .Where(p => p.CityId == cityId && p.Id == poiId)
            .FirstOrDefaultAsync();
        if (pointOfInterest == null)
        {
            throw new PointOfInterestNotFoundException("Id", poiId.ToString());
        }

        return _mapper.Map<PointOfInterestDto>(pointOfInterest);
    }

    public async Task<PointOfInterestDto> CreatePointOfInterestForCity(int cityId, PointOfInterestForCreationDto poi)
    {
        var entry = new PointOfInterest(poi.Name)
        {
            Description = poi.Description,
            CityId = cityId,
        };
        var pointOfInterest = _context.PointsOfInterest
            .Add(entry);
        await _context.SaveChangesAsync();

        var pointOfInterestDto = _mapper.Map<PointOfInterestDto>(pointOfInterest.Entity);
#pragma warning disable CS4014
        _cache.StorePointOfInterest(cityId, pointOfInterestDto);
#pragma warning restore CS4014

        return pointOfInterestDto;
    }

    public async Task UpdatePointOfInterest(int cityId, int poiId, PointOfInterestForUpdateDto poi)
    {
        var pointOfInterest = await _context.PointsOfInterest
            .Where(p => p.CityId == cityId && p.Id == poiId)
            .FirstOrDefaultAsync();
        if (pointOfInterest == null)
        {
            throw new PointOfInterestNotFoundException("Id", poiId.ToString());
        }

        _mapper.Map(poi, pointOfInterest);
        await _context.SaveChangesAsync();
        
        var pointOfInterestDto = _mapper.Map<PointOfInterestDto>(pointOfInterest);
#pragma warning disable CS4014
        _cache.StorePointOfInterest(cityId, pointOfInterestDto);
#pragma warning restore CS4014
    }

    public async Task DeletePointOfInterest(int cityId, int poiId)
    {
        await _context.PointsOfInterest
            .Where(p => p.CityId == cityId && p.Id == poiId)
            .ExecuteDeleteAsync();
        await _context.SaveChangesAsync();
        
#pragma warning disable CS4014
        _cache.DeletePointOfInterest(cityId, poiId);
#pragma warning restore CS4014
    }
}