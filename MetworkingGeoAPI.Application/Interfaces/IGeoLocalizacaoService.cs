using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MetworkingGeoAPI.Domain.Models;

namespace MetworkingGeoAPI.Application.Interfaces
{
    public interface IGeoLocalizacaoService
    {
        public IEnumerable<Geolocalizacao> GetAll(int page);
        public IEnumerable<Geolocalizacao> GetById(Guid idUser);
        public Task Add(LocationEntry pGeo);
        public Task<IEnumerable<Guid>> FindNear(LocationEntry loc);
        public Task<long> GetCount();
    }
}