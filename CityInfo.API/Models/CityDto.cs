using System.ComponentModel.DataAnnotations;

namespace CityInfo.API.Models;

public class CityDto
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    [MaxLength(200)]
    public string? Description { get; set; }
    
    public ICollection<PointOfInterestDto> PointsOfInterest { get; set; } = new List<PointOfInterestDto>();

    public int NumberOfPointsOfInterest => PointsOfInterest.Count;
}