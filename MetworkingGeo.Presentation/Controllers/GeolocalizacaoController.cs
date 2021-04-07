using System;
using System.Collections.Generic;
using MetWorkingGeo.Infra.Interfaces;
using MetworkingGeoAPI.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace MetworkingGeo.Presentation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GeolocalizacaoController
    {
        private readonly IDbGeolocalizacaoMongodb _mongoContext;

        public GeolocalizacaoController(IDbGeolocalizacaoMongodb mongodb)
        {
            _mongoContext = mongodb;
        }

        [HttpGet]
        public IEnumerable<Geolocalizacao> GetAll()
        {
            IEnumerable<Geolocalizacao> lLstGeolocalizacao = new List<Geolocalizacao>();

            lLstGeolocalizacao = _mongoContext.GetContext().Find(x => true).ToList();

            return lLstGeolocalizacao;
        }

        [HttpGet("{idUser}", Name = "GetById")]
        public IEnumerable<Geolocalizacao> GetById(Guid idUser)
        {
            List<Geolocalizacao> lLstGeolocalizacao = new List<Geolocalizacao>();

            lLstGeolocalizacao = _mongoContext.GetContext().Find(x => x.idUser.Equals(idUser)).ToList();

            return lLstGeolocalizacao;
        }

        [HttpPost]
        public void Post([FromBody]Geolocalizacao pGeo)
        {
            _mongoContext.GetContext().InsertOne(pGeo);
        }
    }
}