using System;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;

namespace MetworkingGeoAPI.Domain.Models
{
    public class Geolocalizacao
    {
        [BsonId]
        public Guid id { get; set; }
        [BsonElement("idUser")]
        public Guid idUser { get; set; }

        public DateTime date { get; set; }

        [BsonElement("location")]
        public GeoJsonPoint<GeoJson2DGeographicCoordinates> Location { get; set; }
    }
}