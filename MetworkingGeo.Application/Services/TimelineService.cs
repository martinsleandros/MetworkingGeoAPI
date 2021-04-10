using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MetWorkingGeo.Infra.Interfaces;
using MetworkingGeoAPI.Application.Interfaces;
using MetworkingGeoAPI.Domain.Response;
using MongoDB.Driver;

namespace MetworkingGeoAPI.Application.Services
{
    public class TimelineService : ITimelineService
    {
        private readonly IDbGeolocalizacaoMongodb _mongoContext;
        private readonly IMapper _mapper;

        public TimelineService(IDbGeolocalizacaoMongodb mongodb, IMapper mapper)
        {
            _mongoContext = mongodb;
            _mapper = mapper;
        }

        public List<GeolocalizacaoResponse> GetTimelineUsers(Guid idUser)
        {
            List<GeolocalizacaoResponse> responseList = new();

            var users = _mongoContext.GetContext().Find(x => x.IdUser == idUser).ToList();

            foreach (var user in users)
            {
                
                var geo =  _mapper.Map<GeolocalizacaoResponse>(user);

                responseList.Add(geo);
            }

            return responseList;
        }

    }
}