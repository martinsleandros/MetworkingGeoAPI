using System;
using MetWorkingGeo.API.Models;
using MongoDB.Driver;

namespace MetWorkingGeo.API.Data
{
    public class DBGeolocalizacaoMongodb
    {
        public DBGeolocalizacaoMongodb()
        {
            this.clientMongo = new MongoClient("mongodb+srv://metworking:metworking@clustermetworking.5idnw.mongodb.net/metworking?retryWrites=true&w=majority");
            this.databaseMongo = clientMongo.GetDatabase("Geolocalizacao");

            this.dbGeolocalizacao = databaseMongo.GetCollection<Geolocalizacao>("CollectionGeolocalizacao");
        }

        public MongoClient clientMongo;
        public IMongoDatabase databaseMongo;

        public IMongoCollection<Geolocalizacao> dbGeolocalizacao;

    }
}
