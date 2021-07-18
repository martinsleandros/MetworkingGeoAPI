using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using MetWorkingGeo.Infra.Interfaces;
using MetworkingGeoAPI.Application.Interfaces;
using MetworkingGeoAPI.Domain.Models;
using MetworkingGeoAPI.Domain.Response;
using MongoDB.Driver;

namespace MetworkingGeoAPI.Application.Services
{
    public class TimelineService : ITimelineService
    {
        private readonly IDbTimelineMongodb _mongoContext;
        private readonly IMapper _mapper;
        private readonly HttpClient _httpClient;

        public TimelineService(IDbTimelineMongodb mongodb, IMapper mapper, HttpClient httpClient)
        {
            _mongoContext = mongodb;
            _mapper = mapper;
            _httpClient = httpClient;
        }

        public async Task<List<Friend>> GetTimelineUsers(Guid idUser)
        {
            List<Friend> responseList = new();

            var users = await _mongoContext.GetContext().Find(x => x.IdUser == idUser).FirstOrDefaultAsync();

            if (users != null)
            {
                foreach (var user in users.UsersTimeLine)
                {
                    var friend = new Friend() { idAmigo = user};
                    responseList.Add(friend);
                }
            }

            return responseList;
        }

        public async Task AddToTimeline(Guid user, Guid friend)
        {
            var users = await _mongoContext.GetContext().Find(timeline => timeline.IdUser == user).FirstOrDefaultAsync();

            if (users == null)
            {
                var newTimeLine = new Timeline()
                {
                    IdUser = user,
                    UsersTimeLine = new List<Guid>{friend},
                };
                await _mongoContext.GetContext().InsertOneAsync(newTimeLine);
                return;
            }
            
            var any = users.UsersTimeLine.Any(p => p == friend);

            if (!any)
            {
                users.UsersTimeLine.Add(friend);
                await _mongoContext.GetContext().ReplaceOneAsync(x => x.IdUser == user, users);
            }
        }

        public async Task<ResponseFriendComparison> GetRelationalFriends(Guid userId, List<Guid> usersToCompare)
        {
            var listFriends = new RequestFriend {IdAmigos = usersToCompare};

            var serialized = JsonSerializer.Serialize(listFriends);
            var body = new StringContent(serialized, Encoding.UTF8, "application/json");
            var httpResponse = await _httpClient.PostAsync($"http://localhost:5000/api/v1/UserInterest/interestCompare/{userId}", body);

            if (!httpResponse.IsSuccessStatusCode) return new ResponseFriendComparison()
            {
                isOk = false,
                data = null,
                errors = null
            };

            var readAsStringAsync = await httpResponse.Content.ReadAsStringAsync();
            var responseFriendComparison = JsonSerializer.Deserialize<ResponseFriendComparison>(readAsStringAsync);

            return responseFriendComparison;
        }

        public async Task<ResponseFriendMatchComparison> GetShowTimeLine(Guid userId, RequestMatchFriend usersToCompare)
        {
            var serialized = JsonSerializer.Serialize(usersToCompare);
            var body = new StringContent(serialized, Encoding.UTF8, "application/json");
            var httpResponse = await _httpClient.PostAsync($"http://localhost:5002/api/v1/Match/showTimeline/{userId}", body);

            if (!httpResponse.IsSuccessStatusCode) return new ResponseFriendMatchComparison()
            {
                isOk = false,
                data = null,
                errors = null
            };

            var readAsStringAsync = await httpResponse.Content.ReadAsStringAsync();
            var responseFriendComparison = JsonSerializer.Deserialize<ResponseFriendMatchComparison>(readAsStringAsync);

            return responseFriendComparison;
        }

        public async Task RemoveFromTimeline(Guid firstUser, Guid secondUser)
        {
            var firstUserResponse = await _mongoContext.GetContext().Find(timeline => timeline.IdUser == firstUser).FirstOrDefaultAsync();

            if (firstUserResponse != null)
            {
                var hasUser = firstUserResponse.UsersTimeLine.Any(p => p == secondUser);
                if (hasUser)
                {
                    firstUserResponse.UsersTimeLine.Remove(secondUser);
                }
            }

            await _mongoContext.GetContext().ReplaceOneAsync(x => x.IdUser == firstUser, firstUserResponse);
            
            var secondUserResponse = await _mongoContext.GetContext().Find(timeline => timeline.IdUser == secondUser).FirstOrDefaultAsync();

            if (secondUserResponse != null)
            {
                var hasUser = secondUserResponse.UsersTimeLine.Any(p => p == firstUser);
                if (hasUser)
                {
                    secondUserResponse.UsersTimeLine.Remove(firstUser);
                }
            }
            
            await _mongoContext.GetContext().ReplaceOneAsync(x => x.IdUser == secondUser, secondUserResponse);
        }
    }
}