using CityInfo.API.Models;
using ServiceStack;

namespace CityInfo.API.Services;

/// <inheritdoc />
public class CacheService : Service
{
    private static string GetUniqueKey<T>(int id) where T : IBaseModel
    {
        return UrnId.Create<T>(id.ToString());
    }
    
    private static string GetUniqueKey<T>(int parentId, int id) where T : IBaseModel
    {
        return UrnId.Create<T>($"{parentId}_{id}");
    }

    /// <summary>
    /// Find Point of Interest in cache by id
    /// </summary>
    /// <param name="cityId">ID of the Point of Interest's City</param>
    /// <param name="poiId">Point of Interest ID</param>
    /// <returns>The Point of Interest requested or <value>null</value> if it wasn't found</returns>
    internal Task<PointOfInterestDto?> GetPointOfInterest(int cityId, int poiId)
    {
        return CacheAsync.GetAsync<PointOfInterestDto?>(GetUniqueKey<PointOfInterestDto>(cityId, poiId));
    }

    /// <summary>
    /// Find city in cache by id
    /// </summary>
    /// <param name="cityId">City ID</param>
    /// <returns>The city requested or <value>null</value> if it wasn't found</returns>
    internal Task<CityDto?> GetCity(int cityId)
    {
        return CacheAsync.GetAsync<CityDto?>(GetUniqueKey<CityDto>(cityId));
    }

    /// <summary>
    /// Stores a Point of Interest in the cache storage
    /// </summary>
    /// <param name="cityId">ID of the Point of Interest's City</param>
    /// <param name="pointOfInterest">The Point of Interest to be stored</param>
    /// <returns><value>true</value> of <value>false</value> indicating whether the operation was successful</returns>
    internal Task<bool> StorePointOfInterest(int cityId, PointOfInterestDto pointOfInterest)
    {
        return CacheAsync.SetAsync(GetUniqueKey<PointOfInterestDto>(cityId, pointOfInterest.Id), pointOfInterest);
    }

    /// <summary>
    /// Stores a City in the cache storage
    /// </summary>
    /// <param name="city">The City to be stored</param>
    /// <returns><value>true</value> of <value>false</value> indicating whether the operation was successful</returns>
    internal Task<bool> StoreCity(CityDto city)
    {
        return CacheAsync.SetAsync(GetUniqueKey<CityDto>(city.Id), city);
    }

    /// <summary>
    /// Delete an entry for a Point of Interest from the cache storage
    /// </summary>
    /// <param name="cityId">ID of the Point of Interest's City</param>
    /// <param name="poiId">ID of Point of Interest to be removed</param>
    /// <returns><value>true</value> of <value>false</value> indicating whether the operation was successful</returns>
    internal Task<bool> DeletePointOfInterest(int cityId, int poiId)
    {
        return CacheAsync.RemoveAsync(GetUniqueKey<PointOfInterestDto>(cityId, poiId));
    }

    /// <summary>
    /// Delete an entry for a City from the cache storage
    /// </summary>
    /// <param name="cityId">ID of the City to be removed</param>
    /// <returns><value>true</value> of <value>false</value> indicating whether the operation was successful</returns>
    internal Task<bool> DeleteCity(int cityId)
    {
        return CacheAsync.RemoveAsync(GetUniqueKey<CityDto>(cityId));
    }
}