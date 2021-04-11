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
        
        public async Task<IEnumerable<Geolocalizacao>> GetAll(int page = 0, int total = 20)
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
            var lLstGeolocalizacao = _mongoContext.GetContext().Find(x => x.IdUser.Equals(idUser)).ToList();

            return lLstGeolocalizacao;
        }

        public async Task Add(LocationEntry pGeo)
        {
            var invalidGuid = new Guid("00000000-0000-0000-0000-000000000000");
            if (pGeo.UserId == invalidGuid)
            {
                return;
            }
            
            var geo2 = new Geolocalizacao
            {
                Location = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(
                    new GeoJson2DGeographicCoordinates(pGeo.Longitude, pGeo.Latitude)
                ),
                IdUser = pGeo.UserId,
                Date = ConvertToUtc(pGeo.DateTime),
                CreatedDate = DateTime.UtcNow,
            };
            await _mongoContext.GetContext().InsertOneAsync(geo2);
        }

        public async Task RemoveAll()
        {
            await _mongoContext.GetContext().DeleteManyAsync(FilterDefinition<Geolocalizacao>.Empty);
        }
        
        public async Task<IEnumerable<Guid>> FindNear(LocationEntry loc)
        {
            FieldDefinition<Geolocalizacao> fieldLocation = "location";

            var utcTime = ConvertToUtc(loc.DateTime);

            var timeGreaterMinutes = utcTime.AddMinutes(30);
            var timeLessMinutes = utcTime.AddMinutes(-30);

            var filterPoint = GeoJson.Point(new GeoJson2DCoordinates(loc.Longitude, loc.Latitude));
            var filterNearUsers = Builders<Geolocalizacao>.Filter.NearSphere(fieldLocation, filterPoint, 500);
            var filterUserNotEqualToSameUser = Builders<Geolocalizacao>.Filter.Ne(x => x.IdUser, loc.UserId);
            var filterGreater = Builders<Geolocalizacao>.Filter.Gt(x => x.Date, timeLessMinutes);
            var filterLess = Builders<Geolocalizacao>.Filter.Lt(x => x.Date, timeGreaterMinutes);
            var combineFilters = Builders<Geolocalizacao>.Filter.And(filterNearUsers, filterUserNotEqualToSameUser, filterGreater, filterLess);

            var nearUsersCursor = await _mongoContext.GetContext()
                .DistinctAsync(geolocalizacao => geolocalizacao.IdUser, combineFilters);

            return nearUsersCursor.ToList();
        }
        
        public async Task<IEnumerable<Guid>> FindNearWorker(LocationEntry loc)
        {
            FieldDefinition<Geolocalizacao> fieldLocation = "location";


            var timeGreaterMinutes = loc.DateTime.AddMinutes(30);
            var timeLessMinutes = loc.DateTime.AddMinutes(-30);

            var filterPoint = GeoJson.Point(new GeoJson2DCoordinates(loc.Longitude, loc.Latitude));
            var filterNearUsers = Builders<Geolocalizacao>.Filter.NearSphere(fieldLocation, filterPoint, 500);
            var filterUserNotEqualToSameUser = Builders<Geolocalizacao>.Filter.Ne(x => x.IdUser, loc.UserId);
            var filterGreater = Builders<Geolocalizacao>.Filter.Gt(x => x.Date, timeLessMinutes);
            var filterLess = Builders<Geolocalizacao>.Filter.Lt(x => x.Date, timeGreaterMinutes);
            var combineFilters = Builders<Geolocalizacao>.Filter.And(filterNearUsers, filterUserNotEqualToSameUser, filterGreater, filterLess);

            var nearUsersCursor = await _mongoContext.GetContext()
                .DistinctAsync(geolocalizacao => geolocalizacao.IdUser, combineFilters);

            return nearUsersCursor.ToList();
        }

        public DateTime ConvertToUtc(DateTime dateToConvert)
        {
            DateTimeOffset localServerTime = DateTimeOffset.Now;
            var utcTime = dateToConvert.ToUniversalTime().Subtract(localServerTime.Offset);

            return utcTime;
        }
    }
}