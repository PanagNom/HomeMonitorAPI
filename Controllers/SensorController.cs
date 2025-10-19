using HomeMonitorAPI.Data.Interfaces;
using HomeMonitorAPI.Models;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Sensor?>> Get()
        {
            var reading =  await _sensorRepository.GetSensorData();
            var test =  User.ToString();
            if(reading == null)
            {
                return NotFound();
            }

            return Ok(reading);
        }

        [HttpPost]
        [Route("AddSensorData")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Sensor?>> Add([FromBody ] Sensor sensorData)
        {
            if (sensorData == null) {
                return BadRequest("Sensor data is null.");
            }

            await _sensorRepository.AddSensorAsync(sensorData);

            return Ok();
        }
    }
}
