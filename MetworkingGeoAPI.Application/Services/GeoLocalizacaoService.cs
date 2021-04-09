using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MetWorkingGeo.Infra.Interfaces;
using MetworkingGeoAPI.Application.Interfaces;
using MetworkingGeoAPI.Domain.Models;
using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;
using MongoDB.Driver.Linq;

namespace MetworkingGeoAPI.Application.Services
{
    public class GeoLocalizacaoService : IGeoLocalizacaoService
    {
        private readonly IDbGeolocalizacaoMongodb _mongoContext;

        public GeoLocalizacaoService(IDbGeolocalizacaoMongodb mongodb)
        {
            _mongoContext = mongodb;
        }
        
        public IEnumerable<Geolocalizacao> GetAll()
        {
            IEnumerable<Geolocalizacao> lLstGeolocalizacao = new List<Geolocalizacao>();

            lLstGeolocalizacao = _mongoContext.GetContext().Find(x => true).ToList();

            return lLstGeolocalizacao;
        }
        
        public IEnumerable<Geolocalizacao> GetById(Guid idUser)
        {
            List<Geolocalizacao> lLstGeolocalizacao = new List<Geolocalizacao>();

            lLstGeolocalizacao = _mongoContext.GetContext().Find(x => x.idUser.Equals(idUser)).ToList();

            return lLstGeolocalizacao;
        }

        public async Task Add(LocationEntry pGeo)
        {
            var geo2 = new Geolocalizacao();
            geo2.Location = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(
                new GeoJson2DGeographicCoordinates(pGeo.Longitude, pGeo.Latitude)
            );
            geo2.idUser = pGeo.UserId;
            geo2.date = new DateTime();
            await _mongoContext.GetContext().InsertOneAsync(geo2);
        }
        
        public async Task<IEnumerable<Geolocalizacao>> FindNear(LocationEntry loc)
        {
            FieldDefinition<Geolocalizacao> fieldLocation = "location";
            var filterPoint = GeoJson.Point(new GeoJson2DCoordinates(loc.Longitude, loc.Latitude));
            var filterNear = Builders<Geolocalizacao>.Filter.NearSphere(fieldLocation, filterPoint, 510);
            var filterUser = Builders<Geolocalizacao>.Filter.Ne(x => x.idUser, loc.UserId);
            var combineFilters = Builders<Geolocalizacao>.Filter.And(filterNear, filterUser);

            var teste = _mongoContext.GetContext()
                .Distinct<Geolocalizacao>(new StringFieldDefinition<Geolocalizacao, Geolocalizacao>("userid"), combineFilters);

            return teste.ToList();
        }
    }
}