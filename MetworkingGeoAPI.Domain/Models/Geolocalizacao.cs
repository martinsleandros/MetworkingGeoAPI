using System;
using MongoDB.Bson.Serialization.Attributes;

namespace MetworkingGeoAPI.Domain.Models
{
    public class Geolocalizacao
    {
        [BsonId]
        public Guid id { get; set; }

        public Guid idUser { get; set; }

        public DateTime date { get; set; }

        public decimal latitude { get; set; }

        public decimal longitude { get; set; }
    }
}