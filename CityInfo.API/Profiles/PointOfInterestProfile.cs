using AutoMapper;
using CityInfo.API.Entities;
using CityInfo.API.Models;

namespace CityInfo.API.Profiles;

public class PointOfInterestProfile : Profile
{
    public PointOfInterestProfile()
    {
        CreateMap<PointOfInterest, PointOfInterestDto>();
        CreateMap<PointOfInterestDto, PointOfInterest>()
            .ForMember(dst => dst.Id, cfg => cfg.Ignore())
            .ForMember(dst => dst.CityId, cfg => cfg.Ignore())
            .ForMember(dst => dst.City, cfg => cfg.Ignore());
        CreateMap<PointOfInterestForCreationDto, PointOfInterest>()
            .ForMember(dst => dst.Id, cfg => cfg.Ignore())
            .ForMember(dst => dst.CityId, cfg => cfg.Ignore())
            .ForMember(dst => dst.City, cfg => cfg.Ignore());
        CreateMap<PointOfInterestForUpdateDto, PointOfInterest>()
            .ForMember(dst => dst.Id, cfg => cfg.Ignore())
            .ForMember(dst => dst.CityId, cfg => cfg.Ignore())
            .ForMember(dst => dst.City, cfg => cfg.Ignore());
    }
}
