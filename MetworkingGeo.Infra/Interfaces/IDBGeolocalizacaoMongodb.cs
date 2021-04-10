using MetworkingGeoAPI.Domain.Models;
using MongoDB.Driver;

namespace MetWorkingGeo.Infra.Interfaces
{
    public interface IDbGeolocalizacaoMongodb
    {
        public IMongoCollection<Geolocalizacao> GetContext();
    }
}