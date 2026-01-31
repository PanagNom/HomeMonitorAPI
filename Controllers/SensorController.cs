using HomeMonitorAPI.Data.Interfaces;
using HomeMonitorAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeMonitorAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SensorController(ILogger<SensorController> logger, ISensorRepository sensorRepository) : ControllerBase
    {
        private readonly ILogger<SensorController> logger = logger;
        private readonly ISensorRepository sensorRepository = sensorRepository;

        [HttpGet]
        [Route("GetSensorData")]

        // [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Sensor?>> Get()
        {
            var reading = await this.sensorRepository.GetSensorData();
            _ = this.User.ToString();
            if (reading == null)
            {
                return this.NotFound();
            }

            return this.Ok(reading);
        }

        [HttpPost]
        [Route("AddSensorData")]

        // [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Sensor?>> Add([FromBody] Sensor sensorData)
        {
            if (sensorData == null)
            {
                return this.BadRequest("Sensor data is null.");
            }

            await this.sensorRepository.AddSensorAsync(sensorData);

            return this.Ok();
        }
    }
}