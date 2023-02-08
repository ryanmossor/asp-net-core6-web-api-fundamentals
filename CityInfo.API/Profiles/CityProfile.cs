using AutoMapper;

namespace CityInfo.API.Profiles
{
    public class CityProfile : Profile
    {
        public CityProfile() 
        {
            // map property names on source object to the same property names on the destination object.
            // by default, ignores NullReferenceExceptions from source to target.
            CreateMap<Entities.City, Models.CityWithoutPointsOfInterestDto>();
            CreateMap<Entities.City, Models.CityDto>();
        }
    }
}
