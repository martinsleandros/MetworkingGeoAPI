using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace MetworkingGeoAPI.Domain.Models
{
    public class Timeline
    {
        [BsonId]
        public Guid Id { get; set; }
        [BsonElement("idUser")]
        public Guid IdUser { get; set; }
        public List<Guid> UsersTimeLine { get; set; }
    }
}