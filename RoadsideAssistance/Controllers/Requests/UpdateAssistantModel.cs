using RoadsideAssistance.Models;

namespace RoadsideAssistance.Controllers.Requests
{
    public class UpdateAssistantModel
    {
        public Assistant Assistant { get; set; }
        public Geolocation AssistantLocation { get; set; }
    }
}
