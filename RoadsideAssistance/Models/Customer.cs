using RoadsideAssistance.Models.Interfaces;

namespace RoadsideAssistance.Models
{
    public class Customer : ICustomer
    {
        public string Name { get; set; }
        public Geolocation Location { get; set; }

        public Customer(string name, IGeolocation location)
        {
            Name = name;
            Location = (Geolocation)location;
        }
    
    }
}
