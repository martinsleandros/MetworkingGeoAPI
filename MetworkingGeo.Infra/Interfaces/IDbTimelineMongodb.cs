using MetworkingGeoAPI.Domain.Models;
using MongoDB.Driver;

namespace MetWorkingGeo.Infra.Interfaces
{
    public interface IDbTimelineMongodb
    {
        public IMongoCollection<Timeline> GetContext();
    }
}