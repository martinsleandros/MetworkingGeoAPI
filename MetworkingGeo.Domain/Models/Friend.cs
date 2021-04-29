using System;
using System.Collections.Generic;

namespace MetworkingGeoAPI.Domain.Models
{
    public class Friend
    {
        public Guid idAmigo { get; set; }
    }

    public class RequestFriend
    {
        public List<Guid> IdAmigos { get; set; }
    }
    
    public class ResponseFriend
    {
        public List<Guid> idAmigos { get; set; }
    }
    
    public class RequestMatchFriend
    {
        public List<Friend> IdAmigos { get; set; }
    }
    
    public class ResponseFriendComparison
    {
        public Errors errors { get; set; }
        public bool isOk { get; set; }
        public ResponseFriend data { get; set; }
    }
    
    public class ResponseFriendMatchComparison
    {
        public Errors errors { get; set; }
        public bool isOk { get; set; }
        public List<Friend> data { get; set; }
    }

    public class Errors
    {
        public bool isForbiddens { get; set; }
        public List<string> data { get; set; }
    }
}