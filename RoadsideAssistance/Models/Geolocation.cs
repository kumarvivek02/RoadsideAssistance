using RoadsideAssistance.Models.Interfaces;

namespace RoadsideAssistance.Models
{
    // Geolocation class represents a geographical location
    public class Geolocation : IGeolocation
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public Geolocation(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

    }
}