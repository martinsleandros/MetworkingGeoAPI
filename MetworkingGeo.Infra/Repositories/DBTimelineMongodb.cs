using MetWorkingGeo.Infra.Interfaces;
using MetworkingGeoAPI.Domain.Models;
using MongoDB.Driver;

namespace MetWorkingGeo.Infra.Repositories
{
    public class DBTimelineMongodb : IDbTimelineMongodb
    {
        public DBTimelineMongodb()
        {
            var clientMongo = new MongoClient("mongodb+srv://metworking:metworking@clustermetworking.5idnw.mongodb.net/metworking?retryWrites=true&w=majority");
            var databaseMongo = clientMongo.GetDatabase("Geolocalizacao");
            _dbGeolocalizacao = databaseMongo.GetCollection<Timeline>("CollectionTimeline");
        }
        
        private readonly IMongoCollection<Timeline> _dbGeolocalizacao;

        public IMongoCollection<Timeline> GetContext()
        {
            return _dbGeolocalizacao;
        }
    }
}