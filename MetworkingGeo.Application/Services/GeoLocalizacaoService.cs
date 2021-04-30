using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly ITimelineService _timelineService;

        public GeoLocalizacaoService(IDbGeolocalizacaoMongodb mongodb, ITimelineService timelineService)
        {
            _mongoContext = mongodb;
            _timelineService = timelineService;
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
            await CheckIfHasRelationalAndAdd(geo2);
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
        
        public async Task<List<Guid>> FindNearWorker(LocationEntry loc)
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
        
        private async Task CheckIfHasRelationalAndAdd(Geolocalizacao position)
        {
            var location = new LocationEntry()
            {
                Latitude = position.Location.Coordinates.Latitude,
                Longitude = position.Location.Coordinates.Longitude,
                DateTime = position.Date,
                UserId = position.IdUser
            };
            
            var near = await FindNearWorker(location);
            if (!near.Any()) return;
            
            var relationalFriends = await _timelineService.GetRelationalFriends(position.IdUser, near.ToList());

            if (relationalFriends.isOk && relationalFriends.data != null && relationalFriends.data.idAmigos.Count > 0)
            {
                var friends = relationalFriends.data.idAmigos.Select(requestFriend => new Friend() {idAmigo = requestFriend}).ToList();
                var request = new RequestMatchFriend {IdAmigos = friends};

                var timeLineFriends = await _timelineService.GetShowTimeLine(position.IdUser, request);

                if (timeLineFriends.isOk && timeLineFriends.data != null &&
                    timeLineFriends.data.Count > 0)
                {
                    var friendsGuids = timeLineFriends.data.Select(friend => friend.idAmigo).ToList();

                    foreach (var friend in friendsGuids)
                    {
                        await _timelineService.AddToTimeline(position.IdUser, friend);
                        await _timelineService.AddToTimeline(friend, position.IdUser);
                    }
                }
            }
        }
    }
}