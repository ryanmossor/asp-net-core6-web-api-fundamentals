using CityInfo.API.Entities;

namespace CityInfo.API.Services
{
    public interface ICityInfoRepository
    {
        Task<IEnumerable<City>> GetCitiesAsync(); // can also return IQueryable<City>
        Task<City?> GetCityAsync(int cityId, bool includePointsOfInterest);
        Task<bool> CityExistsAsync(int cityId);
        Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId);
        Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId);
        // This method is only async because it calls to GetCityAsync()
        Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest);
        // Don't need to mark Delete method async since it's an in-memory op, not an I/O op.
        // Also doesn't call any async methods, although you could do this if you wanted. In that case, it needs to be marked async
        void DeletePointOfInterest(PointOfInterest pointOfInterest);
        Task<bool> SaveChangesAsync();
    }
}
