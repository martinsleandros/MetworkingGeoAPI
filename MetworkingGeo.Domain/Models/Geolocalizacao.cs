using System;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;

namespace MetworkingGeoAPI.Domain.Models
{
    public class Geolocalizacao
    {
        [BsonId]
        public Guid Id { get; set; }
        [BsonElement("idUser")]
        public Guid IdUser { get; set; }
        public DateTime Date { get; set; }
        public DateTime CreatedDate { get; set; }

        [BsonElement("location")]
        public GeoJsonPoint<GeoJson2DGeographicCoordinates> Location { get; set; }
    }
}