using RoadsideAssistance.Models;

namespace RoadsideAssistance.Controllers.Requests
{
    public class ReleaseAssistantModel
    {
        public Customer Customer { get; set; }
        public Assistant Assistant { get; set; }
        
    }
}
