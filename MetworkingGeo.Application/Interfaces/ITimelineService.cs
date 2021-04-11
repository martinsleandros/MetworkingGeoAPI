using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MetworkingGeoAPI.Domain.Models;

namespace MetworkingGeoAPI.Application.Interfaces
{
    public interface ITimelineService
    {
        public Task<List<Friend>> GetTimelineUsers(Guid idUser);
        public Task<ResponseFriendComparison> GetRelationalFriends(Guid userId, List<Friend> usersToCompare);
        public Task<ResponseFriendComparison> GetShowTimeLine(Guid userId, List<Friend> usersToCompare);
        public Task AddToTimeline(Guid user, List<Guid> friends);
    }
}
