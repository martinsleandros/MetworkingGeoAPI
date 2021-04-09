using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MetWorkingGeo.Infra.Interfaces;
using MetworkingGeoAPI.Application.Interfaces;
using MetworkingGeoAPI.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace MetworkingGeo.Presentation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GeolocalizacaoController : ControllerBase
    {
        private readonly IGeoLocalizacaoService _geoLocalizacaoService;

        public GeolocalizacaoController(IGeoLocalizacaoService geoLocalizacaoService)
        {
            _geoLocalizacaoService = geoLocalizacaoService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IEnumerable<Geolocalizacao> lLstGeolocalizacao = new List<Geolocalizacao>();

            lLstGeolocalizacao = _geoLocalizacaoService.GetAll();

            return Ok(lLstGeolocalizacao);
        }

        [HttpGet("{idUser}", Name = "GetById")]
        public IEnumerable<Geolocalizacao> GetById(Guid idUser)
        {
            List<Geolocalizacao> lLstGeolocalizacao = new List<Geolocalizacao>();

            lLstGeolocalizacao = _geoLocalizacaoService.GetById(idUser).ToList();

            return lLstGeolocalizacao;
        }

        [HttpPost]
        public async void Post([FromBody]LocationEntry pGeo)
        {
            await _geoLocalizacaoService.Add(pGeo);
        }
        
        [HttpPost("/near")]
        public async Task<IActionResult> GetNear([FromBody]LocationEntry pGeo)
        {
            var result = await _geoLocalizacaoService.FindNear(pGeo);
            return Ok(result);
        }
    }
}