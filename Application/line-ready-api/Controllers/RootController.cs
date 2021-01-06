using LineReadyApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace LineReadyApi.Controllers
{
    [Route("/")]
    [ApiController]
    [ApiVersion("1.0")]
    public class RootController : ControllerBase
    {
        /// <summary>
        /// Shows the Swagger Interface [html]
        /// </summary>
        [HttpGet(Name = nameof(GetRoot))]
        [ProducesResponseType(200)]
        public IActionResult GetRoot()
        {
            var response = new
            {
                Self = Link.To(nameof(GetRoot)),
                Fish = Link.To(nameof(FishController.GetFish)),
                Cities = Link.ToCollection(nameof(CitiesController.GetCities))
            };

            return Ok(response);
        }
    }
}