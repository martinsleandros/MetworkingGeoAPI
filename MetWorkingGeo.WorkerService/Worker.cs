using System;
using System.Threading;
using System.Threading.Tasks;
using MetworkingGeoAPI.Application.Interfaces;
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
                var all = _service.GetAll(_offset);
                _logger.LogInformation($"Worker running. Offset is: {_offset.ToString()}", DateTimeOffset.Now);
                _offset += 1;
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}