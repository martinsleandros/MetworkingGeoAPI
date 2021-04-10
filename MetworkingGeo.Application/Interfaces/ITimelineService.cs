using MetworkingGeoAPI.Domain.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MetworkingGeoAPI.Application.Interfaces
{
    public interface ITimelineService
    {
        public List<GeolocalizacaoResponse> GetTimelineUsers(Guid idUser);
    }
}
