using MetWorkingGeo.Infra.Interfaces;
using MetworkingGeoAPI.Domain.Models;
using MongoDB.Driver;

namespace MetWorkingGeo.Infra.Repositories
{
    public class DBTimelineMongodb : IDbGeolocalizacaoMongodb
    {
        public DBTimelineMongodb()
        {
            var clientMongo = new MongoClient("mongodb+srv://metworking:metworking@clustermetworking.5idnw.mongodb.net/metworking?retryWrites=true&w=majority");
            var databaseMongo = clientMongo.GetDatabase("Geolocalizacao");
            _dbGeolocalizacao = databaseMongo.GetCollection<Geolocalizacao>("CollectionGeolocalizacao");
        }
        
        private readonly IMongoCollection<Geolocalizacao> _dbGeolocalizacao;

        public IMongoCollection<Geolocalizacao> GetContext()
        {
            return _dbGeolocalizacao;
        }
    }
}