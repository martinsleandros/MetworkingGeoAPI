using System.Threading;
using System.Threading.Tasks;
using MetworkingGeoAPI.Application.Interfaces;
using MetworkingGeoAPI.Domain.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MetWorkingGeo.WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IGeoLocalizacaoService _service;
        private int _offset = 0;
        public Worker(ILogger<Worker> logger, IGeoLocalizacaoService service)
        {
            _logger = logger;
            _service = service;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var total = await _service.GetCount();
                const int totalToReturn = 100;
                var totalPages = total / totalToReturn;
                var all = await _service.GetAll(_offset, totalToReturn);

                foreach (var position in all)
                {
                    var location = new LocationEntry()
                    {
                        Latitude = position.Location.Coordinates.Latitude,
                        Longitude = position.Location.Coordinates.Longitude,
                        DateTime = position.Date,
                        UserId = position.IdUser
                    };
                    
                    var near = await _service.FindNearWorker(location);
                }

                if (_offset > totalPages)
                {
                    _offset = 0;
                }
                else
                {
                    _offset += 1;
                }
                
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}