using System;
using System.Collections.Generic;
using System.Linq;
using MetWorkingGeo.Infra.Interfaces;
using MetworkingGeoAPI.Application.Interfaces;
using MetworkingGeoAPI.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace MetworkingGeo.Presentation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GeolocalizacaoController
    {
        private readonly IGeoLocalizacaoService _geoLocalizacaoService;

        public GeolocalizacaoController(IGeoLocalizacaoService geoLocalizacaoService)
        {
            _geoLocalizacaoService = geoLocalizacaoService;
        }

        [HttpGet]
        public IEnumerable<Geolocalizacao> GetAll()
        {
            IEnumerable<Geolocalizacao> lLstGeolocalizacao = new List<Geolocalizacao>();

            lLstGeolocalizacao = _geoLocalizacaoService.GetAll();

            return lLstGeolocalizacao;
        }

        [HttpGet("{idUser}", Name = "GetById")]
        public IEnumerable<Geolocalizacao> GetById(Guid idUser)
        {
            List<Geolocalizacao> lLstGeolocalizacao = new List<Geolocalizacao>();

            lLstGeolocalizacao = _geoLocalizacaoService.GetById(idUser).ToList();

            return lLstGeolocalizacao;
        }

        [HttpPost]
        public void Post([FromBody]Geolocalizacao pGeo)
        {
            _geoLocalizacaoService.Add(pGeo);
        }
    }
}