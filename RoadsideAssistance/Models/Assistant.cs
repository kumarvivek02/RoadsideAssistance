using RoadsideAssistance.Models.Interfaces;

namespace RoadsideAssistance.Models
{
    public class Assistant :IAssistant
    {
        public string Name { get; set; }
        public Geolocation Location { get; set; }
        public bool IsReserved { get; set; }
        public DateTime LastAssignmentEndTime { get; set; }
        public DateTime LastAssignmentStartTime { get; set; }

        public Assistant(string name, IGeolocation location)
        {
            Name = name;
            Location = (Geolocation)location;
            IsReserved = false;
        }

        public int CompareTo(Assistant other)
        {
            // Implement comparison logic based on distance calculation           
            double distanceToOther = CalculateDistance(Location, other.Location);
            double distanceToSelf = CalculateDistance(Location, Location);

            return distanceToSelf.CompareTo(distanceToOther);
        }

        private double CalculateDistance(Geolocation location1, Geolocation location2)
        {            
            // Keeping it simple, let's assume we are calculating straight-line distance
            double latDiff = location2.Latitude - location1.Latitude;
            double lonDiff = location2.Longitude - location1.Longitude;
            double distance = Math.Sqrt(latDiff * latDiff + lonDiff * lonDiff);

            return distance;
        }
    }
}