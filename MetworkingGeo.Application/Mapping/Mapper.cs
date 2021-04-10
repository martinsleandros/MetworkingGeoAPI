using AutoMapper;
using MetworkingGeoAPI.Domain.Models;
using MetworkingGeoAPI.Domain.Response;

namespace MetworkingGeoAPI.Application.Mapping
{
    public class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<Geolocalizacao, GeolocalizacaoResponse>();
        }

    }
}
