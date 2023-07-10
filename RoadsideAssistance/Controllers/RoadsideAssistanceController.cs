using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using RoadsideAssistance.Controllers.Requests;
using RoadsideAssistance.Models;
using RoadsideAssistance.Services;
using RoadsideAssistance.Services.Interfaces;
using System.Web.Http;
using FromBodyAttribute = Microsoft.AspNetCore.Mvc.FromBodyAttribute;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace RoadsideAssistance.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoadsideAssistanceController : ControllerBase
    {
        private readonly RoadsideAssistanceService _roadsideAssistanceService;

        public RoadsideAssistanceController(IRoadsideAssistanceService roadsideAssistanceService)
        {
            _roadsideAssistanceService = (RoadsideAssistanceService)roadsideAssistanceService;
        }

        [HttpPost("/assistant/location")]
        public IActionResult UpdateAssistantLocation([FromBody] UpdateAssistantModel updateModel)
        {
            Assistant assistant = updateModel.Assistant;
            Geolocation assistantLocation = updateModel.AssistantLocation;
            if (assistantLocation != null && assistant != null) {
                _roadsideAssistanceService.UpdateAssistantLocation(assistant, assistantLocation);
                return Ok(); 
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet("/assistants")]
        public IActionResult FindNearestAssistants([FromBody] Geolocation geolocation, int limit = 5)
        {            
            SortedSet<Assistant> nearestAssistants = _roadsideAssistanceService.FindNearestAssistants(geolocation, limit);
            return Ok(nearestAssistants);
        }

        [HttpPost("/assistant/reserve")]
        public IActionResult ReserveAssistant([FromBody] ReserveAssistantModel reserveModel)
        {
            Customer customer = reserveModel.Customer;
            Geolocation customerLocation = reserveModel.CustomerLocation;
            Optional<Assistant> reservedAssistant = _roadsideAssistanceService.ReserveAssistant(customer, customerLocation);
            if (reservedAssistant.HasValue)
            {
                return Ok(reservedAssistant.Value);
            }
            return NotFound();
        }

        [HttpPost("/assistant/release")]
        public IActionResult ReleaseAssistant([FromBody] ReleaseAssistantModel releaseAssistantModel  )
        {
            Customer customer = releaseAssistantModel.Customer;
            Assistant assistant = releaseAssistantModel.Assistant;
            _roadsideAssistanceService.ReleaseAssistant(customer, assistant);
            return Ok();
        }
    }
}
