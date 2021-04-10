using System;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;

namespace MetworkingGeoAPI.Domain.Response
{
    public class GeolocalizacaoResponse
    {
        public Guid IdUser { get; set; }
        public DateTime Date { get; set; }
        public DateTime CreatedDate { get; set; }
        public GeoJsonPoint<GeoJson2DGeographicCoordinates> Location { get; set; }
    }
}