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
        private readonly ITimelineService _timelineService;

        public GeolocalizacaoController(IGeoLocalizacaoService geoLocalizacaoService, ITimelineService timelineService)
        {
            _geoLocalizacaoService = geoLocalizacaoService;
            _timelineService = timelineService;
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

        [HttpGet("/timeline/{idUser}")]
        public async Task<IActionResult> GetTimelineUsers(Guid idUser)
        {
            var result = await _timelineService.GetTimelineUsers(idUser);
            return Ok(result);
        }

        [HttpDelete("/{firstUser}/{secondUser}")]
        public async Task<IActionResult> RemoveFromTimeline([FromRoute] Guid firstUser, [FromRoute] Guid secondUser)
        {
            await _timelineService.RemoveFromTimeline(firstUser, secondUser);

            return Ok();
        }
    }
}