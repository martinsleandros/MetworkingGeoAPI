using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MetworkingGeoAPI.Application.Interfaces;
using MetworkingGeoAPI.Domain.Models;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> GetAll([FromQuery] int page, [FromQuery] int total)
        {
            var lLstGeolocalizacao = await _geoLocalizacaoService.GetAll(page, total);

            return Ok(lLstGeolocalizacao);
        }
        
        [HttpDelete]
        public async Task<IActionResult> DeleteAll()
        {
            await _geoLocalizacaoService.RemoveAll();

            return Ok();
        }

        [HttpGet("{idUser}", Name = "GetById")]
        public IEnumerable<Geolocalizacao> GetById(Guid idUser)
        {
            var lLstGeolocalizacao = _geoLocalizacaoService.GetById(idUser).ToList();

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