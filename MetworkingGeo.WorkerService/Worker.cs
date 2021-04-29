using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly ITimelineService _timelineService;
        private int _offset = 0;
        public Worker(ILogger<Worker> logger, IGeoLocalizacaoService service, ITimelineService timelineService)
        {
            _logger = logger;
            _service = service;
            _timelineService = timelineService;
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
                    if (!near.Any()) continue;
                    var friendsList = near.Select(user => new Friend() {idAmigo = user,}).ToList();
                    var relationalFriends = await _timelineService.GetRelationalFriends(position.IdUser, near.ToList());

                    if (relationalFriends.isOk && relationalFriends.data != null && relationalFriends.data.idAmigos.Count > 0)
                    {
                        var friends = relationalFriends.data.idAmigos.Select(requestFriend => new Friend() {idAmigo = requestFriend}).ToList();
                        var request = new RequestMatchFriend {IdAmigos = friends};

                        var timeLineFriends = await _timelineService.GetShowTimeLine(position.IdUser, request);

                        if (timeLineFriends.isOk && timeLineFriends.data != null &&
                            timeLineFriends.data.Count > 0)
                        {
                            var guids = timeLineFriends.data.Select(friend => friend.idAmigo).ToList();
                            
                            await _timelineService.AddToTimeline(position.IdUser, guids);
                        }
                    }
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