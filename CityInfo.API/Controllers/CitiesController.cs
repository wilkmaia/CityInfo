using CityInfo.API.Exceptions;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace CityInfo.API.Controllers;

[Authorize]
[ApiController]
[Route("api/cities")]
public class CitiesController : ControllerBase
{
    private readonly CityInfoRepository _cityInfoRepository;
    private const int MaxPageSize = 20;

    public CitiesController(CityInfoRepository cityInfoRepository)
    {
        _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerator<CityDto>>> GetCities(
        [FromQuery(Name = "nameFilter")] string? nameFilter,
        [FromQuery(Name = "searchQuery")] string? searchQuery,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10
    )
    {
        if (pageNumber <= 0)
        {
            ModelState.AddModelError(nameof(pageNumber), "Page number should be a positive integer");
        }

        if (pageSize <= 0)
        {
            ModelState.AddModelError(nameof(pageSize), "Page size should be a positive integer");
        }

        if (ModelState.ErrorCount > 0)
        {
            return ValidationProblem();
        }
        
        int normalizedPageSize = new int[] { pageSize, MaxPageSize }.Min();
        var (cities, paginationMetadata) = await _cityInfoRepository.GetCities(nameFilter, searchQuery, pageNumber, normalizedPageSize);
        Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));
        return Ok(cities);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CityDto>> GetCity(int id)
    {
        try
        {
            CityDto city = await _cityInfoRepository.GetCityById(id);
            return Ok(city);
        }
        catch (CityNotFoundException)
        {
            return NotFound();
        }
    }
}