using MetWorkingGeo.Infra.Interfaces;
using MetworkingGeoAPI.Domain.Models;
using MongoDB.Driver;

namespace MetWorkingGeo.Infra.Repositories
{
    public class DbGeolocalizacaoMongodb : IDbGeolocalizacaoMongodb
    {
        public DbGeolocalizacaoMongodb()
        {
            this.clientMongo = new MongoClient("mongodb+srv://metworking:metworking@clustermetworking.5idnw.mongodb.net/metworking?retryWrites=true&w=majority");
            this.databaseMongo = clientMongo.GetDatabase("Geolocalizacao");

            this.dbGeolocalizacao = databaseMongo.GetCollection<Geolocalizacao>("CollectionGeolocalizacao");
        }

        public MongoClient clientMongo;
        public IMongoDatabase databaseMongo;

        public IMongoCollection<Geolocalizacao> dbGeolocalizacao;

        public IMongoCollection<Geolocalizacao> GetContext()
        {
            return this.dbGeolocalizacao;
        }
    }
}