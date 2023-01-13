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

    public CityInfoRepository(CityInfoContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<List<CityDto>> GetAllCities()
    {
        return _mapper.Map<List<CityDto>>(await _context.Cities
            .Include(c => c.PointsOfInterest)
            .AsNoTracking()
            .ToListAsync());
    }

    public async Task<CityDto> GetCityById(int cityId, bool track = false)
    {
        var query = track ? _context.Cities : _context.Cities.AsNoTracking();
        var city = await query
            .Include(c => c.PointsOfInterest)
            .SingleOrDefaultAsync(c => c.Id == cityId);
        if (city == null)
        {
            throw new CityNotFoundException("Id", cityId.ToString());
        }

        return _mapper.Map<CityDto>(city);
    }

    public async Task<List<PointOfInterestDto>> GetAllPointsOfInterestForCity(int cityId)
    {
        var city = await _context.Cities
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == cityId);
        if (city == null)
        {
            throw new CityNotFoundException("Id", cityId.ToString());
        }

        return _mapper.Map<List<PointOfInterestDto>>(city.PointsOfInterest.ToList());
    }

    public async Task<PointOfInterestDto> GetPointOfInterestById(int poiId)
    {
        var pointOfInterest = await _context.PointsOfInterest
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == poiId);
        if (pointOfInterest == null)
        {
            throw new PointOfInterestNotFoundException("Id", poiId.ToString());
        }

        return _mapper.Map<PointOfInterestDto>(pointOfInterest);
    }

    public async Task<PointOfInterestDto> GetPointOfInterestForCityById(int cityId, int poiId, bool track = false)
    {
        var query = track ? _context.Cities : _context.Cities.AsNoTracking();
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

        return _mapper.Map<PointOfInterestDto>(pointOfInterest.Entity);
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
    }

    public async Task DeletePointOfInterest(int cityId, int poiId)
    {
        await _context.PointsOfInterest
            .Where(p => p.CityId == cityId && p.Id == poiId)
            .ExecuteDeleteAsync();
        await _context.SaveChangesAsync();
    }
}