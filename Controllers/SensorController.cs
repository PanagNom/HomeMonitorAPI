using HomeMonitorAPI.Data.Interfaces;
using HomeMonitorAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace HomeMonitorAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SensorController : ControllerBase
    {
        private readonly ILogger<SensorController> _logger;
        private readonly ISensorRepository _sensorRepository;
        public SensorController(ILogger<SensorController> logger, ISensorRepository sensorRepository)
        {
            _logger = logger;
            _sensorRepository = sensorRepository;
        }

        [HttpGet]
        [Route("GetSensorData")]
        public async Task<ActionResult<Sensor?>> Get()
        {
            var reading =  await _sensorRepository.GetSensorData();

            if(reading == null)
            {
                return NotFound();
            }

            return Ok(reading);
        }
    }
}
