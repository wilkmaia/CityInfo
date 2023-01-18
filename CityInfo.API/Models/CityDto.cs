using System.ComponentModel.DataAnnotations;

namespace CityInfo.API.Models;

/// <summary>
/// A DTO for a City
/// </summary>
public class CityDto : IBaseModel
{
    /// <summary>
    /// City name
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// City description
    /// </summary>
    [MaxLength(200)]
    public string? Description { get; set; }
    
    /// <summary>
    /// List of city's points of interest
    /// </summary>
    public ICollection<PointOfInterestDto> PointsOfInterest { get; set; } = new List<PointOfInterestDto>();

    /// <summary>
    /// Number of city's points of interest
    /// </summary>
    public int NumberOfPointsOfInterest => PointsOfInterest.Count;
}