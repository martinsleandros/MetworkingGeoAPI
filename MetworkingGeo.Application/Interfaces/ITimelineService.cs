using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MetworkingGeoAPI.Domain.Models;

namespace MetworkingGeoAPI.Application.Interfaces
{
    public interface ITimelineService
    {
        public Task<List<Friend>> GetTimelineUsers(Guid idUser);
        public Task<ResponseFriendComparison> GetRelationalFriends(Guid userId, List<Guid> usersToCompare);
        public Task<ResponseFriendMatchComparison> GetShowTimeLine(Guid userId, RequestMatchFriend usersToCompare);
        public Task AddToTimeline(Guid user, Guid friends);
        public Task RemoveFromTimeline(Guid firstUser, Guid secondUser);
    }
}
