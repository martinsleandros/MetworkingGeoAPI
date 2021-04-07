using System;
using System.Collections.Generic;
using MetworkingGeoAPI.Domain.Models;

namespace MetworkingGeoAPI.Application.Interfaces
{
    public interface IGeoLocalizacaoService
    {
        public IEnumerable<Geolocalizacao> GetAll();
        public IEnumerable<Geolocalizacao> GetById(Guid idUser);
        public void Add(Geolocalizacao pGeo);
    }
}