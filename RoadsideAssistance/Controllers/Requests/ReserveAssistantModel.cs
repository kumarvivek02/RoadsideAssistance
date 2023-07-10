using RoadsideAssistance.Models;

namespace RoadsideAssistance.Controllers.Requests
{
    public class ReserveAssistantModel
    {        
        public Customer Customer { get; set; }
        public Geolocation CustomerLocation { get; set; }
    }
}
