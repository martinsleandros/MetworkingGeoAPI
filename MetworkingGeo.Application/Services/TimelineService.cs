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

        public async Task AddToTimeline(Guid user, List<Guid> friends)
        {
            var users = await _mongoContext.GetContext().Find(timeline => timeline.IdUser == user).FirstOrDefaultAsync();

            if (users == null)
            {
                var newTimeLine = new Timeline()
                {
                    IdUser = user,
                    UsersTimeLine = friends,
                };
                await _mongoContext.GetContext().InsertOneAsync(newTimeLine);
                return;
            }

            var needsReplace = false;

            foreach (var newFriend in friends)
            {
                var any = users.UsersTimeLine.Any(p => p == newFriend);

                if (!any)
                {
                    needsReplace = true;
                    users.UsersTimeLine.Add(newFriend);
                }
            }

            if (needsReplace)
            {
                await _mongoContext.GetContext().ReplaceOneAsync(x => x.IdUser == user, users);
            }
        }

        public async Task<ResponseFriendComparison> GetRelationalFriends(Guid userId, List<Friend> usersToCompare)
        {
            var listFriends = new RequestFriend()
            {
                IdAmigos = usersToCompare,
            };
            var serialized = JsonSerializer.Serialize(listFriends);
            var body = new StringContent(serialized, Encoding.UTF8, "application/json");
            var httpResponse = await _httpClient.PostAsync($"http://localhost:8081/api/v1/UserInterest/interestCompare/{userId}", body);

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

        //[Produces("application/json")]
        public async Task<ResponseFriendComparison> GetShowTimeLine(Guid userId, List<Friend> usersToCompare)
        {
            var listFriends = new RequestFriend()
            {
                IdAmigos = usersToCompare,
            };
            var serialized = JsonSerializer.Serialize(listFriends);
            var body = new StringContent(serialized, Encoding.UTF8, "application/json");
            var httpResponse = await _httpClient.PostAsync($"http://localhost:8082/api/v1/Match/showTimeline/{userId}", body);

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
            
            var secondUserResponse = await _mongoContext.GetContext().Find(timeline => timeline.IdUser == secondUser).FirstOrDefaultAsync();

            if (secondUserResponse != null)
            {
                var hasUser = secondUserResponse.UsersTimeLine.Any(p => p == firstUser);
                if (hasUser)
                {
                    secondUserResponse.UsersTimeLine.Remove(firstUser);
                }
            }
        }

    }
}