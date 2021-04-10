using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MetWorkingGeo.Infra.Interfaces;
using MetworkingGeoAPI.Application.Interfaces;
using MetworkingGeoAPI.Domain.Models;
using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;

namespace MetworkingGeoAPI.Application.Services
{
    public class GeoLocalizacaoService : IGeoLocalizacaoService
    {
        private readonly IDbGeolocalizacaoMongodb _mongoContext;

        public GeoLocalizacaoService(IDbGeolocalizacaoMongodb mongodb)
        {
            _mongoContext = mongodb;
        }
        
        public async Task<IEnumerable<Geolocalizacao>> GetAll(int page, int total = 100)
        {
            var lLstGeolocalizacao = _mongoContext.GetContext().Find(x => true).Skip(page).Limit(total).ToList();

            return lLstGeolocalizacao;
        }
        
        public async Task<long> GetCount()
        {
            var lLstGeolocalizacao = await _mongoContext.GetContext().Find(x => true).CountDocumentsAsync();

            return lLstGeolocalizacao;
        }
        
        public IEnumerable<Geolocalizacao> GetById(Guid idUser)
        {
            var lLstGeolocalizacao = _mongoContext.GetContext().Find(x => x.idUser.Equals(idUser)).ToList();

            return lLstGeolocalizacao;
        }

        public async Task Add(LocationEntry pGeo)
        {
            var geo2 = new Geolocalizacao
            {
                Location = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(
                    new GeoJson2DGeographicCoordinates(pGeo.Longitude, pGeo.Latitude)
                ),
                idUser = pGeo.UserId,
                date = pGeo.DateTime,
            };
            await _mongoContext.GetContext().InsertOneAsync(geo2);
        }
        
        public async Task<IEnumerable<Guid>> FindNear(LocationEntry loc)
        {
            FieldDefinition<Geolocalizacao> fieldLocation = "location";

            var timeGreaterMinutes = loc.DateTime.AddMinutes(30);
            var minutesLessMinutes = loc.DateTime.AddMinutes(-30).Minute;
            var timeLessBetween = new DateTime(loc.DateTime.Year, loc.DateTime.Month, loc.DateTime.Day, loc.DateTime.Hour, minutesLessMinutes, loc.DateTime.Second);
            
            
            var filterPoint = GeoJson.Point(new GeoJson2DCoordinates(loc.Longitude, loc.Latitude));
            var filterNearUsers = Builders<Geolocalizacao>.Filter.NearSphere(fieldLocation, filterPoint, 500);
            var filterUserNotEqualToSameUser = Builders<Geolocalizacao>.Filter.Ne(x => x.idUser, loc.UserId);
            var filterGreater = Builders<Geolocalizacao>.Filter.Gt(x => x.date, timeGreaterMinutes);
            var filterLess = Builders<Geolocalizacao>.Filter.Lt(x => x.date, timeLessBetween);
            var combineFilters = Builders<Geolocalizacao>.Filter.And(filterNearUsers, filterUserNotEqualToSameUser, filterGreater, filterLess);

            var nearUsersCursor = await _mongoContext.GetContext()
                .DistinctAsync(geolocalizacao => geolocalizacao.idUser, combineFilters);

            return nearUsersCursor.ToList();
        }
    }
}