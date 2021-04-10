using System;

namespace MetworkingGeoAPI.Domain.Models
{
    public class LocationEntry
    {
        public Guid UserId { get; set; }
        public DateTime DateTime { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}