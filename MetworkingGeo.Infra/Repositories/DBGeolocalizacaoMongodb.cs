using System;
using MetWorkingGeo.Infra.Interfaces;
using MetworkingGeoAPI.Domain.Models;
using MongoDB.Driver;

namespace MetWorkingGeo.Infra.Repositories
{
    public class DbGeolocalizacaoMongodb : IDbGeolocalizacaoMongodb
    {
        public DbGeolocalizacaoMongodb()
        {
            var clientMongo = new MongoClient("mongodb+srv://metworking:metworking@clustermetworking.5idnw.mongodb.net/metworking?retryWrites=true&w=majority");
            var databaseMongo = clientMongo.GetDatabase("Geolocalizacao");
            _dbGeolocalizacao = databaseMongo.GetCollection<Geolocalizacao>("CollectionGeolocalizacao");
            CreateIndex();
            CreateExpiration();
        }

        private readonly IMongoCollection<Geolocalizacao> _dbGeolocalizacao;
        public IMongoCollection<Geolocalizacao> GetContext()
        {
            return _dbGeolocalizacao;
        }

        private void CreateIndex()
        {
            var indexKeysDefinition = Builders<Geolocalizacao>.IndexKeys.Geo2DSphere("location");
            _dbGeolocalizacao.Indexes.CreateOne(new CreateIndexModel<Geolocalizacao>(indexKeysDefinition));
        }

        private void CreateExpiration()
        {
            var expirationKey = Builders<Geolocalizacao>.IndexKeys.Ascending("CreatedDate");
            _dbGeolocalizacao.Indexes.CreateOne(expirationKey, new CreateIndexOptions()
            {
                ExpireAfter = new TimeSpan(24, 0, 0)
            });
        }
    }
}